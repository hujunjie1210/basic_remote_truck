using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace logisteering
{
    public class logi
    {
        public  void init()
        {
            LogitechGSDK.LogiSteeringInitialize(false);
        }
        public void update()
        {
            //All the test functions are called on the first device plugged in(index = 0)   
            if (LogitechGSDK.LogiUpdate() && LogitechGSDK.LogiIsConnected(0))
            {
                if (LogitechGSDK.LogiIsPlaying(0, LogitechGSDK.LOGI_FORCE_SPRING))
                {
                    LogitechGSDK.LogiStopSpringForce(0);
                }
                else
                {
                    LogitechGSDK.LogiPlaySpringForce(0, 50, 50, 50);
                }
            }
        }
        public void Stop()
        {
            LogitechGSDK.LogiSteeringShutdown();
        }


        /// <summary>
        /// 读取方向盘转角
        /// </summary>
        /// <returns>[-450, 450], 左正右负</returns>
        public double SteeringWheelAngle()
        {
            if (!LogitechGSDK.LogiUpdate() || !LogitechGSDK.LogiIsConnected(0) || LogitechGSDK.LogiGetStateENGINES(0) == IntPtr.Zero)
                return 0;

            double ret = -450 * (double)(LogitechGSDK.LogiGetStateCSharp(0).lX) / 32767;

            return ret;
        }

        /// <summary>
        /// 油门开度范围
        /// </summary>
        /// <returns>0-99</returns>
        public int accelerator_range()
        {
            if (!LogitechGSDK.LogiUpdate() || !LogitechGSDK.LogiIsConnected(0) || LogitechGSDK.LogiGetStateENGINES(0) == IntPtr.Zero)
                return 0;

            int ret = -100 * (LogitechGSDK.LogiGetStateCSharp(0).lY-32767) /65536 ;

            return ret;
        }

        /// <summary>
        /// 刹车开度范围
        /// </summary>
        /// <returns>0-99</returns>
        public int brake_range()
        {
            if (!LogitechGSDK.LogiUpdate() || !LogitechGSDK.LogiIsConnected(0) || LogitechGSDK.LogiGetStateENGINES(0) == IntPtr.Zero)
                return 0;

            int ret = -99 * (LogitechGSDK.LogiGetStateCSharp(0).lRz - 32767) / 65536;

            return ret;
        }

        public bool change_car1()
        {
            if (!LogitechGSDK.LogiUpdate() || !LogitechGSDK.LogiIsConnected(0) || LogitechGSDK.LogiGetStateENGINES(0) == IntPtr.Zero)
                return false;

            return LogitechGSDK.LogiGetStateCSharp(0).rgbButtons[5] == 128;
        }
        public bool change_car2()
        {
            if (!LogitechGSDK.LogiUpdate() || !LogitechGSDK.LogiIsConnected(0) || LogitechGSDK.LogiGetStateENGINES(0) == IntPtr.Zero)
                return false;

            return LogitechGSDK.LogiGetStateCSharp(0).rgbButtons[12] == 128;
        }
        public bool change_car3()
        {
            if (!LogitechGSDK.LogiUpdate() || !LogitechGSDK.LogiIsConnected(0) || LogitechGSDK.LogiGetStateENGINES(0) == IntPtr.Zero)
                return false;

            return LogitechGSDK.LogiGetStateCSharp(0).rgbButtons[6] != 0;
        }

        public int switch_car()
        {
            if (!LogitechGSDK.LogiUpdate() || !LogitechGSDK.LogiIsConnected(0) || LogitechGSDK.LogiGetStateENGINES(0) == IntPtr.Zero)
                return 0;

            int ret=0;

            if (LogitechGSDK.LogiGetStateCSharp(0).rgdwPOV[0] == 0)
            {
                ret = 1;
            }
                
            if (LogitechGSDK.LogiGetStateCSharp(0).rgdwPOV[0] == 9000)
            {
                ret = 2;
            }
                
            if (LogitechGSDK.LogiGetStateCSharp(0).rgdwPOV[0] == 18000)
            {
                ret = 3;
            }
                
            if (LogitechGSDK.LogiGetStateCSharp(0).rgdwPOV[0] == 27000)
            {
                ret = 4;
            }
                

            return ret;
        }

        public bool cam_monitor()
        {
            if (!LogitechGSDK.LogiUpdate() || !LogitechGSDK.LogiIsConnected(0) || LogitechGSDK.LogiGetStateENGINES(0) == IntPtr.Zero)
                return false;

            return LogitechGSDK.LogiGetStateCSharp(0).rgbButtons[0] != 0;

        }

        public bool take_control()
        {
            if (!LogitechGSDK.LogiUpdate() || !LogitechGSDK.LogiIsConnected(0) || LogitechGSDK.LogiGetStateENGINES(0) == IntPtr.Zero)
                return false;

            return LogitechGSDK.LogiGetStateCSharp(0).rgbButtons[23] != 0;

        }
        public bool exchange()
        {
            if (!LogitechGSDK.LogiUpdate() || !LogitechGSDK.LogiIsConnected(0) || LogitechGSDK.LogiGetStateENGINES(0) == IntPtr.Zero)
                return false;

            return LogitechGSDK.LogiGetStateCSharp(0).rgbButtons[11] != 0;
        }
        public bool gps_loc()
        {
            if (!LogitechGSDK.LogiUpdate() || !LogitechGSDK.LogiIsConnected(0) || LogitechGSDK.LogiGetStateENGINES(0) == IntPtr.Zero)
                return false;

            return LogitechGSDK.LogiGetStateCSharp(0).rgbButtons[7] != 0;
        }
        public int car_gear()
        {
            int ret = 0;
            if (!LogitechGSDK.LogiUpdate() || !LogitechGSDK.LogiIsConnected(0) || LogitechGSDK.LogiGetStateENGINES(0) == IntPtr.Zero)
                ret = 3;

            if (LogitechGSDK.LogiGetStateCSharp(0).rgbButtons[14] != 0)
                ret = 4;

            if (LogitechGSDK.LogiGetStateCSharp(0).rgbButtons[15] != 0)
                ret = 2;

            return ret;

        }

        public int control_sign()
        {
            int ret = 0;
            if (!LogitechGSDK.LogiUpdate() || !LogitechGSDK.LogiIsConnected(0) || LogitechGSDK.LogiGetStateENGINES(0) == IntPtr.Zero)
                ret = 0;

            if (LogitechGSDK.LogiGetStateCSharp(0).rgbButtons[23] != 0)
                ret = 1;

            if (LogitechGSDK.LogiGetStateCSharp(0).rgbButtons[11] != 0)
                ret = 0;

            return ret;
        }

        public bool escape()
        {
            if (!LogitechGSDK.LogiUpdate() || !LogitechGSDK.LogiIsConnected(0) || LogitechGSDK.LogiGetStateENGINES(0) == IntPtr.Zero)
                return false;

            return LogitechGSDK.LogiGetStateCSharp(0).rgbButtons[24] != 0;
        }
    }
}
