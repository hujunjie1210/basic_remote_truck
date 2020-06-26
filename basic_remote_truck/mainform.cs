using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using RCComm;
using CamWidget;
using logisteering;
using DES_Sharp;
using System.IO;
using System.Net.NetworkInformation;
using System.Windows.Forms.DataVisualization.Charting;
using System.Collections;
using System.Security.Permissions;
using System.Xml;


namespace basic_remote_truck
{
    [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]
    public partial class mainform : Form
    {
       
        //调用方向盘的类
        logi hjj = new logi();

        CamMngr mngr = new CamMngr();

        object[] ob = new object[2];

        public static object LockData = new object();
        public static object LockData1 = new object();
        public static object LockData2 = new object();
        public static object LockData3 = new object();
        public static object LockData4 = new object();

        DGramRcver<UplinkDGram> rcver = new DGramRcver<UplinkDGram>();
        DgramSender<DownlinkDGram> sd = new DgramSender<DownlinkDGram>();
        UplinkDGram obj = new UplinkDGram();
        DownlinkDGram data = new DownlinkDGram();

        DGramRcver<UplinkDGram> rcver2 = new DGramRcver<UplinkDGram>();
        DgramSender<DownlinkDGram> sd2 = new DgramSender<DownlinkDGram>();
        UplinkDGram obj2 = new UplinkDGram();
        //DownlinkDGram data2 = new DownlinkDGram();

        DGramRcver<UplinkDGram> rcver3 = new DGramRcver<UplinkDGram>();
        DgramSender<DownlinkDGram> sd3 = new DgramSender<DownlinkDGram>();
        UplinkDGram obj3 = new UplinkDGram();
        //DownlinkDGram data3 = new DownlinkDGram();

        public mainform()
        {  
            
            InitializeComponent();
            chartsetting();

            //导航网页调取
            IEupdate.SetWebBrowserFeatures(11);
            webBrowser1.ScriptErrorsSuppressed = true;
            webBrowser1.Url = new Uri(Path.Combine(Application.StartupPath, "mapbaidu.html"));
            webBrowser1.ObjectForScripting = this;


            //初始化方向盘并更新
            hjj.init();
            hjj.update();

           
           //初始化相机并调用
            CamMngr.Init();
            mngr.FromXml("../config/cam.xml");
            mngr.DoLoginAll();
            Thread.Sleep(100);
            mngr.DoPlayAll();
            
            camPlayerWnd1.Visible = false;
            camPlayerWnd2.Visible = false;
            camPlayerWnd3.Visible = false;
            camPlayerWnd4.Visible = false;
            camPlayerWnd5.Visible = false;
            camPlayerWnd6.Visible = false;


            // 启动Rcver线程，参数为本地接收端口
            //rcver.Start(8115);
            //rcver2.Start(8215);
            //rcver3.Start(8315);


            //启动send线程
            //sd.Start("192.168.1.164", 8110);
            //sd2.Start("192.168.1.164", 8210);
            //sd3.Start("192.168.1.164", 8310);

            //多车接受  需要判断address_source

            //启动三辆车Rcver线程
            recv_car_info(1);
            recv_car_info(2);
            recv_car_info(3);

            //获取车名
            get_car_name(1);
            get_car_name(2);
            get_car_name(3);

        }
        private void mainform_Load(object sender, EventArgs e)
        {
            draw();
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Start();
            timer2.Tick += new EventHandler(timer2_Tick);
            timer2.Start();
            timer4.Tick += new EventHandler(NETTIME_Tick);
            timer4.Start();

        }

        string ip_addr_recv;
        int port;
        int port_rec;
        string car_name;
        private void get_car_name(int car_id)
        {
            XmlDocument doc_car = new XmlDocument();
            doc_car.Load(@"../config/carname.xml");
            XmlNode xn = doc_car.SelectSingleNode("cars");
            XmlNodeList xnl = xn.ChildNodes;

            foreach (XmlNode xn1 in xnl)
            {
                XmlElement xe = (XmlElement)xn1;
                int car = 0;
                bool id_juage = int.TryParse(xe.GetAttribute("id"), out car);
                if (car == car_id)
                {
                    car_name = xe.GetAttribute("name");
                    if (car == 1)
                    {
                        name1.Text = car_name;
                    }
                    if (car == 2)
                    {
                        name2.Text = car_name;
                    }
                    if (car == 3)
                    {
                        name3.Text = car_name;
                    }
                }
            }
        }
        private void recv_car_info(int car_id)
        {
            XmlDocument doc_car = new XmlDocument();
            doc_car.Load(@"../config/carname.xml");
            XmlNode xn = doc_car.SelectSingleNode("cars");
            XmlNodeList xnl = xn.ChildNodes;

            bool port_rec_ok;
            foreach (XmlNode xn1 in xnl)
            {
                XmlElement xe = (XmlElement)xn1;
                int car = 0;
                bool id_juage = int.TryParse(xe.GetAttribute("id"), out car);
                if (car == car_id)
                {
                    port_rec_ok = int.TryParse(xe.GetAttribute("port_rec"), out port_rec);
                    if (car == 1)
                    {
                        rcver.Start(port_rec);
                    }
                    if (car == 2)
                    {
                        rcver2.Start(port_rec);
                    }
                    if (car == 3)
                    {
                        rcver3.Start(port_rec);
                    }
                }
            }
        }

        private void send_car_info(int car_id)
        {
            XmlDocument doc_car = new XmlDocument();
            doc_car.Load(@"../config/carname.xml");
            XmlNode xn = doc_car.SelectSingleNode("cars");
            XmlNodeList xnl = xn.ChildNodes;

            bool port_ok;
            foreach (XmlNode xn1 in xnl)
            {
                XmlElement xe = (XmlElement)xn1;
                int car = 0;
                bool id_juage = int.TryParse(xe.GetAttribute("id"), out car);
                if (car == car_id)
                {
                    ip_addr_recv = xe.GetAttribute("ip");
                    port_ok = int.TryParse(xe.GetAttribute("port"), out port);
                    if (car == 1)
                    {
                        sd.Start(ip_addr_recv, port);
                    }
                    if (car == 2)
                    {
                        sd2.Start(ip_addr_recv, port);
                    }
                    if (car == 3)
                    {
                        sd3.Start(ip_addr_recv, port);
                    }
                }
            }
        }

        string get_ip;
        private void get_ipstr(int car_id)
        {
            XmlDocument doc_car = new XmlDocument();
            doc_car.Load(@"../config/carname.xml");
            XmlNode xn = doc_car.SelectSingleNode("cars");
            XmlNodeList xnl = xn.ChildNodes;
            foreach (XmlNode xn1 in xnl)
            {
                XmlElement xe = (XmlElement)xn1;
                int car = 0;
                bool id_juage = int.TryParse(xe.GetAttribute("id"), out car);
                if (car == car_id)
                {
                    get_ip = xe.GetAttribute("ip");
                }
            }
        }




        private void timer1_Tick(object sender, EventArgs e)
        {
            lock (LockData2)
            {
                // FetchData为线程外异步获取数据的接口
                rcver.FetchData(ref obj);

                label7.Text = obj.m_vehicle_state.ToString();
                speed.Text = obj.m_speed.ToString();
                label10.Text = obj.m_rpm.ToString();
                label2.Text = obj.m_steering.ToString();
                label3.Text = obj.m_path_heading.ToString();
                label4.Text = ((double)obj.m_path_x / 1000000).ToString();
                label5.Text = ((double)obj.m_path_y / 1000000).ToString();

                // FetchData为线程外异步获取数据的接口
                rcver2.FetchData(ref obj2);

                label36.Text = obj2.m_vehicle_state.ToString();
                label38.Text = obj2.m_speed.ToString();
                label40.Text = obj2.m_rpm.ToString();
                label43.Text = obj2.m_steering.ToString();
                label39.Text = obj2.m_path_heading.ToString();
                label44.Text = ((double)obj2.m_path_x / 1000000).ToString();
                label45.Text = ((double)obj2.m_path_y / 1000000).ToString();



                // FetchData为线程外异步获取数据的接口
                rcver3.FetchData(ref obj3);

                label47.Text = obj3.m_vehicle_state.ToString();
                label46.Text = obj3.m_speed.ToString();
                label48.Text = obj3.m_rpm.ToString();
                label51.Text = obj3.m_steering.ToString();
                label52.Text = obj3.m_path_heading.ToString();
                label53.Text = ((double)obj3.m_path_x / 1000000).ToString();
                label54.Text = ((double)obj3.m_path_y / 1000000).ToString();

            }

        }
        
        private void cam1_open()
        {
            camPlayerWnd1.Visible = true;
            camPlayerWnd2.Visible = true;
            camPlayerWnd3.Visible = false;
            camPlayerWnd4.Visible = false;
            camPlayerWnd5.Visible = false;
            camPlayerWnd6.Visible = false;
        }

        private void cam2_open()
        {
            camPlayerWnd1.Visible = false;
            camPlayerWnd2.Visible = false;
            camPlayerWnd3.Visible = true;
            camPlayerWnd4.Visible = true;
            camPlayerWnd5.Visible = false;
            camPlayerWnd6.Visible = false;
        }

        private void cam3_open()
        {
            camPlayerWnd1.Visible = false;
            camPlayerWnd2.Visible = false;
            camPlayerWnd3.Visible = false;
            camPlayerWnd4.Visible = false;
            camPlayerWnd5.Visible = true;
            camPlayerWnd6.Visible = true;
        }
        private bool Flag_change()
        {
            if (label1.Text == " " && steering.Text == " " && label49.Text == " " && label50.Text == " " && label41.Text == " " && label42.Text == " ")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private void timer2_Tick(object sender, EventArgs e)
        {
            if (Flag_change() == true)
            {
                //判断当前位置
                if (pictureBox6.Visible == true && pictureBox7.Visible == false && pictureBox8.Visible == false)//如果位置在1
                {
                    if (hjj.switch_car() == 2)
                    {
                        pictureBox6.Visible = false;
                        pictureBox7.Visible = true;
                        pictureBox8.Visible = false;
                    }
                    if (hjj.switch_car() == 4)
                    {
                        pictureBox6.Visible = false;
                        pictureBox7.Visible = false;
                        pictureBox8.Visible = true;
                    }
                   if (hjj.cam_monitor() == true)
                    {
                        cam1_open();
                    }
                    if (hjj.take_control() == true)
                    {
                        control2.Stop();
                        control3.Stop();
                        Thread.Sleep(20);
                        control1.Tick += new EventHandler(timer3_Tick);
                        control1.Start();
                    }
                    if (hjj.gps_loc() == true)
                    {
                        map2.Stop();
                        map3.Stop();
                        Thread.Sleep(20);
                        map1.Tick += new EventHandler(map1_Tick);
                        map1.Start();
                    }
                    Thread.Sleep(10);
                }
                else if (pictureBox6.Visible == false && pictureBox7.Visible == true && pictureBox8.Visible == false)//如果位置在2
                {
                    if (hjj.switch_car() == 2)
                    {
                        pictureBox6.Visible = false;
                        pictureBox7.Visible = false;
                        pictureBox8.Visible = true;
                    }
                    if (hjj.switch_car() == 4)
                    {
                        pictureBox6.Visible = true;
                        pictureBox7.Visible = false;
                        pictureBox8.Visible = false;
                    }
                   if (hjj.cam_monitor() == true)
                    {
                        cam2_open();
                    }
                    if (hjj.take_control() == true)
                    {
                        control1.Stop();
                        control3.Stop();
                        Thread.Sleep(20);
                        control2.Tick += new EventHandler(control2_Tick);
                        control2.Start();
                    }
                    if (hjj.gps_loc() == true)
                    {
                        map2.Stop();
                        map3.Stop();
                        Thread.Sleep(20);
                        map1.Tick += new EventHandler(map1_Tick);
                        map1.Start();
                    }
                    Thread.Sleep(10);
                }
                else if (pictureBox6.Visible == false && pictureBox7.Visible == false && pictureBox8.Visible == true)//如果位置在3
                {
                    if (hjj.switch_car() == 2)
                    {
                        pictureBox6.Visible = true;
                        pictureBox7.Visible = false;
                        pictureBox8.Visible = false;
                    }
                    if (hjj.switch_car() == 4)
                    {
                        pictureBox6.Visible = false;
                        pictureBox7.Visible = true;
                        pictureBox8.Visible = false;
                    }
                   if (hjj.cam_monitor() == true)
                    {
                        cam3_open();
                    }
                    if (hjj.take_control() == true)
                    {
                        control1.Stop();
                        control2.Stop();
                        Thread.Sleep(20);
                        control3.Tick += new EventHandler(control3_Tick);
                        control3.Start();
                    }
                    if (hjj.gps_loc() == true)
                    {
                        map2.Stop();
                        map3.Stop();
                        Thread.Sleep(20);
                        map1.Tick += new EventHandler(map1_Tick);
                        map1.Start();
                    }
                    Thread.Sleep(10);
                }
            }
        }
        private void map1_Tick(object sender, EventArgs e)
        {
            //电子地图通信
            ob[0] = (double)obj.m_path_x / 1000000;
            ob[1] = (double)obj.m_path_y / 1000000;
            webBrowser1.Document.InvokeScript("setpos", ob);
            if (hjj.exchange() == true)
            {
                map1.Stop();
                map2.Stop();
                map3.Stop();
            }
        }

        private void map2_Tick(object sender, EventArgs e)
        {
            //电子地图通信
            ob[0] = (double)obj2.m_path_x / 1000000;
            ob[1] = (double)obj2.m_path_y / 1000000;
            webBrowser1.Document.InvokeScript("setpos", ob);
            if (hjj.exchange() == true)
            {
                map1.Stop();
                map2.Stop();
                map3.Stop();
            }
        }

        private void map3_Tick(object sender, EventArgs e)
        {
            //电子地图通信
            ob[0] = (double)obj3.m_path_x / 1000000;
            ob[1] = (double)obj3.m_path_y / 1000000;
            webBrowser1.Document.InvokeScript("setpos", ob);
            if (hjj.exchange() == true)
            {
                map1.Stop();
                map2.Stop();
                map3.Stop();
            }
        }

        private void timer3_Tick(object sender, EventArgs e)
        {
            steering.Text = hjj.SteeringWheelAngle().ToString();
            label1.Text = hjj.accelerator_range().ToString();
            data.m_rc_steer = (int)hjj.SteeringWheelAngle();
            data.m_rc_acc_pedal = (short)hjj.accelerator_range();
            data.m_rc_brake_pedal = (short)hjj.brake_range();
            data.m_rc_gear = (short)hjj.car_gear();
            data.m_ctrl_sign = 1;

            sd.UpdateData(data);

            if (hjj.exchange() == true)
            {
                data.m_rc_steer = 0;
                data.m_rc_acc_pedal = 0;
                data.m_rc_brake_pedal = 0;
                data.m_rc_gear = 3;
                data.m_ctrl_sign = 0;
                sd.UpdateData(data);
                control1.Stop();
                control2.Stop();
                control3.Stop();
                map1.Stop();
                map2.Stop();
                map3.Stop();
                steering.Text = " ";
                label1.Text = " ";
            }
            label41.Text = " ";
            label42.Text = " ";
            label49.Text = " ";
            label50.Text = " ";

            if (data.m_rc_gear == 3)
            {
                P.Visible = false;
                R.Visible = false;
                N.Visible = true;
                D.Visible = false;
            }
            else if (data.m_rc_gear == 2)
            {
                P.Visible = false;
                R.Visible = true;
                N.Visible = false;
                D.Visible = false;
            }
            else if (data.m_rc_gear == 4)
            {
                P.Visible = false;
                R.Visible = false;
                N.Visible = false;
                D.Visible = true;
            }
            else
            {
                P.Visible = false;
                R.Visible = false;
                N.Visible = true;
                D.Visible = false;
            }

            dashboard1.Real = obj.m_rpm / 80;
            label55.Text = obj.m_speed.ToString();
        }
        private void control2_Tick(object sender, EventArgs e)
        {
            label42.Text = hjj.SteeringWheelAngle().ToString();
            label41.Text = hjj.accelerator_range().ToString();
            data.m_rc_steer = (int)hjj.SteeringWheelAngle();
            data.m_rc_acc_pedal = (short)hjj.accelerator_range();
            data.m_rc_brake_pedal = (short)hjj.brake_range();
            data.m_rc_gear = (short)hjj.car_gear();
            data.m_ctrl_sign = 1;

            sd2.UpdateData(data);

            if (hjj.exchange() == true)
            {
                data.m_rc_steer = 0;
                data.m_rc_acc_pedal = 0;
                data.m_rc_brake_pedal = 0;
                data.m_rc_gear = 3;
                data.m_ctrl_sign = 0;
                sd2.UpdateData(data);
                control1.Stop();
                control2.Stop();
                control3.Stop();
                map1.Stop();
                map2.Stop();
                map3.Stop();
                label41.Text = " ";
                label42.Text = " ";
            }
            steering.Text = " ";
            label1.Text = " ";
            label49.Text = " ";
            label50.Text = " ";

            if (data.m_rc_gear == 3)
            {
                P.Visible = false;
                R.Visible = false;
                N.Visible = true;
                D.Visible = false;
            }
            else if (data.m_rc_gear == 2)
            {
                P.Visible = false;
                R.Visible = true;
                N.Visible = false;
                D.Visible = false;
            }
            else if (data.m_rc_gear == 4)
            {
                P.Visible = false;
                R.Visible = false;
                N.Visible = false;
                D.Visible = true;
            }
            else
            {
                P.Visible = false;
                R.Visible = false;
                N.Visible = true;
                D.Visible = false;
            }
            dashboard1.Real = obj2.m_rpm / 80;
            label55.Text = obj2.m_speed.ToString();
        }
        private void control3_Tick(object sender, EventArgs e)
        {
            label50.Text = hjj.SteeringWheelAngle().ToString();
            label49.Text = hjj.accelerator_range().ToString();
            data.m_rc_steer = (int)hjj.SteeringWheelAngle();
            data.m_rc_acc_pedal = (short)hjj.accelerator_range();
            data.m_rc_brake_pedal = (short)hjj.brake_range();
            data.m_rc_gear = (short)hjj.car_gear();
            data.m_ctrl_sign = 1;

            sd3.UpdateData(data);

            if (hjj.exchange() == true)
            {
                data.m_rc_steer = 0;
                data.m_rc_acc_pedal = 0;
                data.m_rc_brake_pedal = 0;
                data.m_rc_gear = 3;
                data.m_ctrl_sign = 0;
                sd3.UpdateData(data);
                control1.Stop();
                control2.Stop();
                control3.Stop();
                map1.Stop();
                map2.Stop();
                map3.Stop();
                label50.Text = " ";
                label49.Text = " ";
            }
            steering.Text = " ";
            label1.Text = " ";
            label41.Text = " ";
            label42.Text = " ";

            if (data.m_rc_gear == 3)
            {
                P.Visible = false;
                R.Visible = false;
                N.Visible = true;
                D.Visible = false;
            }
            else if (data.m_rc_gear == 2)
            {
                P.Visible = false;
                R.Visible = true;
                N.Visible = false;
                D.Visible = false;
            }
            else if (data.m_rc_gear == 4)
            {
                P.Visible = false;
                R.Visible = false;
                N.Visible = false;
                D.Visible = true;
            }
            else
            {
                P.Visible = false;
                R.Visible = false;
                N.Visible = true;
                D.Visible = false;
            }
            dashboard1.Real = obj2.m_rpm / 80;
            label55.Text = obj2.m_speed.ToString();
        }


        Ping pingSender = new Ping();
        string[] ipstr = new string[3];
        short suggest_speed;
        string tdata = "a";
        int timeout = 1000;
        private void NETTIME_Tick(object sender, EventArgs e)
        {
            for (int i = 1; i <= 3; i++)
            {
                get_ipstr(i);
                ipstr[i - 1] = get_ip;
            }
            byte[] buffer = Encoding.ASCII.GetBytes(tdata);
            //Ping 选项设置  
            PingOptions options1 = new PingOptions();
            PingOptions options2 = new PingOptions();
            PingOptions options3 = new PingOptions();
            options1.DontFragment = true;
            options2.DontFragment = true;
            options3.DontFragment = true;

            PingReply reply1 = pingSender.Send(ipstr[0], timeout, buffer, options1);
            PingReply reply2 = pingSender.Send(ipstr[1], timeout, buffer, options2);
            PingReply reply3 = pingSender.Send(ipstr[2], timeout, buffer, options3);

            if (reply1.Status == IPStatus.Success)
            {
                label58.Text = "连接成功";
                label60.Text = reply1.RoundtripTime.ToString() + "ms";

            }
            else if (reply1.Status == IPStatus.TimedOut)
            {
                label58.Text = "连接超时";
            }
            else
            {
                label58.Text = "连接失败";
            }
            date.Text = DateTime.Now.ToLongTimeString();
            label56.Text = DateTime.Now.ToLongDateString();
            
            long range1 = reply1.RoundtripTime;
            long range2 = reply2.RoundtripTime;
            long range3 = reply3.RoundtripTime;

            Series NET = chart1.Series[0];
            Series NET2 = chart1.Series[1];
            Series NET3 = chart1.Series[2];
            NET.Name = name1.Text;
            NET2.Name = name2.Text;
            NET3.Name = name3.Text;


            DateTime time = DateTime.Now;
            NET.Points.AddXY(time, range1 + 1);
            NET2.Points.AddXY(time, range2 + 1);
            NET3.Points.AddXY(time, range3 + 1);

            chart1.ChartAreas[0].AxisX.ScaleView.Position = NET.Points.Count - 5;

            suggest_speed = (short)(20 - range1 * 0.055);
            label62.Text = suggest_speed.ToString() + "km/h";
            
        }


        private void chartsetting()
        {
            chart1.ChartAreas[0].AxisX.LabelStyle.Format = "HH:mm:ss";
            chart1.ChartAreas[0].AxisX.ScaleView.Size = 3; 
            chart1.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true ;
            chart1.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
        }

        private void draw()
        {
            P.Parent = pictureBox1;
            R.Parent = pictureBox1;
            D.Parent = pictureBox1;
            N.Parent = pictureBox1;
            label7.Parent = pictureBox1;
            name3.Parent = pictureBox1;
            label9.Parent = pictureBox1;
            label10.Parent = pictureBox1;
            label11.Parent = pictureBox1;
            label12.Parent = pictureBox1;
            label13.Parent = pictureBox1;
            label14.Parent = pictureBox1;
            label15.Parent = pictureBox1;
            label16.Parent = pictureBox1;
            label17.Parent = pictureBox1;
            label18.Parent = pictureBox1;
            label19.Parent = pictureBox1;
            label20.Parent = pictureBox1;
            label21.Parent = pictureBox1;
            label22.Parent = pictureBox1;
            label23.Parent = pictureBox1;
            label24.Parent = pictureBox1;
            label25.Parent = pictureBox1;
            label26.Parent = pictureBox1;
            label27.Parent = pictureBox1;
            label28.Parent = pictureBox1;
            label29.Parent = pictureBox1;
            label30.Parent = pictureBox1;
            label31.Parent = pictureBox1;
            label32.Parent = pictureBox1;
            label33.Parent = pictureBox1;
            label34.Parent = pictureBox1;
            label35.Parent = pictureBox1;
            label36.Parent = pictureBox1;
            label37.Parent = pictureBox1;
            label38.Parent = pictureBox1;
            label39.Parent = pictureBox1;
            label40.Parent = pictureBox1;
            label41.Parent = pictureBox1;
            label42.Parent = pictureBox1;
            label43.Parent = pictureBox1;
            label44.Parent = pictureBox1;
            label45.Parent = pictureBox1;
            label46.Parent = pictureBox1;
            label47.Parent = pictureBox1;
            label48.Parent = pictureBox1;
            label49.Parent = pictureBox1;
            label50.Parent = pictureBox1;
            label51.Parent = pictureBox1;
            label52.Parent = pictureBox1;
            label53.Parent = pictureBox1;
            label54.Parent = pictureBox1;
            label55.Parent = pictureBox1;
            label57.Parent = pictureBox1;
            label58.Parent = pictureBox1;
            label59.Parent = pictureBox1;
            label60.Parent = pictureBox1;
            label61.Parent = pictureBox1;
            label62.Parent = pictureBox1;
            name1.Parent = pictureBox1;
            name2.Parent = pictureBox1;
            speed.Parent = pictureBox1;
            steering.Parent = pictureBox1;
            label1.Parent = pictureBox1;
            label2.Parent = pictureBox1;
            label3.Parent = pictureBox1;
            label4.Parent = pictureBox1;
            label5.Parent = pictureBox1;
            name2.Parent = pictureBox1;

            dashboard1.Parent = pictureBox1;
            dashboardpetit1.Parent = pictureBox1;
            date.Parent = pictureBox1;
            label56.Parent = pictureBox1;
            chart1.Parent = pictureBox1;

            pictureBox6.Parent = pictureBox1;
            pictureBox7.Parent = pictureBox1;
            pictureBox8.Parent = pictureBox1;
            pictureBox6.Visible = true;
            pictureBox7.Visible = false;
            pictureBox8.Visible = false;

            pictureBox5.BackgroundImage = Image.FromFile("../bin/image/logo.jpg");
            pictureBox2.BackgroundImage = Image.FromFile("../bin/image/car1.jpg");
            pictureBox3.BackgroundImage = Image.FromFile("../bin/image/car2.jpg");
            pictureBox4.BackgroundImage = Image.FromFile("../bin/image/car3.jpg");

            camPlayerWnd1.Parent = pictureBox1;
            camPlayerWnd2.Parent = pictureBox1;
            camPlayerWnd3.Parent = pictureBox1;
            camPlayerWnd4.Parent = pictureBox1;
            camPlayerWnd5.Parent = pictureBox1;
            camPlayerWnd6.Parent = pictureBox1;
            camPlayerWnd1.Visible = false;
            camPlayerWnd2.Visible = false;
            camPlayerWnd3.Visible = false;
            camPlayerWnd4.Visible = false;
            camPlayerWnd5.Visible = false;
            camPlayerWnd6.Visible = false;

            P.Visible = false;
            R.Visible = false;
            N.Visible = false;
            D.Visible = false;
        }

       
    }
}
