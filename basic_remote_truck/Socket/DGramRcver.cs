using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace RCComm
{
    /// <summary>
    /// 接收报文并转换成结构体T的对象
    /// </summary>
    /// <typeparam name="T">报文结构体类型</typeparam>
    class DGramRcver<T>
    {
        // ------------------------------------------------------
        // 变量区
        // ------------------------------------------------------
        private Socket m_socket;                            // socket
        private EndPoint m_remote_ep;                       // 发送端信息，由IP和端口定义
        private int m_local_port;                           // 本地用于接收的端口
        public T m_data;                                    // 预备接收数据的类型T的结构体对象
        private Thread m_thread;                            // 接收线程
        private readonly object data_lock = new object();   // 对 m_data 操作的互斥锁

        /// <summary>
        /// 构造函数
        /// </summary>
        public DGramRcver()
        {
            m_remote_ep = new IPEndPoint(IPAddress.Any, 0);
            //m_remote_ep = new IPEndPoint(IPAddress.Parse("10.173.0.2"), 8111);
        }

        // ------------------------------------------------------
        // 私有方法区
        // ------------------------------------------------------
        /// <summary>
        /// Socket 接收报文的主体函数，用于线程体的循环中
        /// </summary>
        /// <returns>接收字节数</returns>
        private int Receive()
        {
            int data_size = Marshal.SizeOf(typeof(T));
            byte[] buffer = new byte[1024];
            int length = 0;

            try
            {
                length = m_socket.ReceiveFrom(buffer, ref m_remote_ep);
                Console.WriteLine("{0} bytes received.", length);

            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
            }

            if (length <= 0 || length != data_size)
                //Console.WriteLine("length:{0}", length);
                //Console.WriteLine("data_size:{0}", data_size);
                return length;

            lock(data_lock)
            {
                m_data = BytesConverter.BytesToStruct<T>(buffer);
            }

            return length;
        }

        /// <summary>
        /// 线程体函数
        /// </summary>
        private void ThreadBody()
        {
            m_socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            try
            {
                m_socket.Bind(new IPEndPoint(IPAddress.Any, m_local_port));
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
            m_socket.ReceiveTimeout = 2000;

            while (Thread.CurrentThread.IsAlive)
            {
                Receive();
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
                    Console.WriteLine("Tring to Abort the receiving thread...");
                    Thread.Sleep(100);
                }
            }
        }

        // ------------------------------------------------------
        // 对外方法区
        // ------------------------------------------------------
        /// <summary>
        /// 启动接收线程
        /// </summary>
        /// <param name="_local_port">本地监听端口</param>
        public void Start(int _local_port)
        {
            if (m_socket != null)
                m_socket.Close();

            AbortThread();

            m_local_port = _local_port;

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
        /// 提供给线程外使用，获取 m_data 的当前值
        /// </summary>
        /// <param name="_out_data">[out ref] T 类型的引用参数，用于外部获取 m_data 的值</param>
        public void FetchData(ref T _out_data)
        {
            lock(data_lock)
            {
                _out_data = m_data;
            }
        }
    }
}
