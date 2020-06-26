using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace RCComm
{
    /// <summary>
    /// 将结构体作为udp报文发送
    /// </summary>
    /// <typeparam name="T">结构体类型</typeparam>
    class DgramSender<T>
    {
        // ------------------------------------------------------
        // 变量区
        // ------------------------------------------------------
        private Socket m_socket;                            // socket
        private EndPoint m_remote_ep;                       // 接收端信息，由IP和端口定义
        
        private T m_data;                                   // 用于缓存待发送结构体数值的对象，由互斥锁控制
        private Thread m_thread;                            // 发送线程
        private readonly object data_lock = new object();   // 对 m_data 操作的互斥锁

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="_period">[in] 发送间隔，单位毫秒，默认1秒</param>
        

        // ------------------------------------------------------
        // 私有方法区
        // ------------------------------------------------------
        /// <summary>
        /// Socket 发送报文的主体函数，用于线程体的循环中
        /// </summary>
        /// <param name="_obj">待发送的结构体对象</param>
        /// <returns>发送成功：字节数；发送失败：错误代码</returns>
        private int Send(T _obj)
        {
            byte[] to_send;
            int bytes_sent = 0;

            lock (data_lock)
            {
                to_send = BytesConverter.StructToBytes<T>(_obj);
            }
            
            int size = Marshal.SizeOf(typeof(T));
            if (size <= 0)
            {
                Console.WriteLine("Data length is 0.");
                return 0;
            }

            try
            {
                bytes_sent = m_socket.SendTo(to_send, m_remote_ep);
                Console.WriteLine("{0} Bytes sent.", bytes_sent);
            }
            catch(System.Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return bytes_sent;
        }

        /// <summary>
        /// 发送线程主体，作为参数传递给线程初始化过程
        /// </summary>
        private void ThreadBody()
        {
            m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            while (Thread.CurrentThread.IsAlive)
            {
                if (m_data != null)
                    Send(m_data);

                Thread.Sleep(20);
            }
        }

        /// <summary>
        /// 退出线程的过程
        /// </summary>
        private void AbortThread()
        {
            if (m_thread != null && m_thread.ThreadState != ThreadState.Aborted)
            {
                m_thread.Abort();

                while (m_thread.ThreadState != ThreadState.Aborted)
                {
                    Console.WriteLine("Tring to Abort the sending thread...");
                    Thread.Sleep(100);
                }
            }
        }
        /// <summary>
        /// 设置接收端参数
        /// </summary>
        /// <param name="_ip">接收端的IP地址</param>
        /// <param name="_port">接收端口</param>
        private void SetRemoteEndpoint(string _ip, int _port)
        {
            m_remote_ep = new IPEndPoint(IPAddress.Parse(_ip), _port);
        }

        // ------------------------------------------------------
        // 对外方法区
        // ------------------------------------------------------
        /// <summary>
        /// 启动发送线程
        /// </summary>
        /// <param name="_ip">接收端IP</param>
        /// <param name="_port">接收端端口</param>
        public void Start(string _ip, int _port)
        {
            if (m_socket != null)
                m_socket.Close();

            AbortThread();

            SetRemoteEndpoint(_ip, _port);
            m_thread = new Thread(ThreadBody);
            m_thread.Start();
        }

        /// <summary>
        /// 结束线程并释放资源
        /// </summary>
        public void Stop()
        {
            if (m_socket != null)
                m_socket.Close();

            AbortThread();
        }

        /// <summary>
        /// 提供给线程外使用，更新 m_data 的值
        /// </summary>
        /// <param name="_in_obj">[in] 作为拷贝对象的结构体对象</param>
        public void UpdateData(T _in_obj)
        {
            lock (data_lock)
            {
                m_data = _in_obj;
            }
        }
    }
}
