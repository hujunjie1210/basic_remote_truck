using System;

namespace DES_Sharp
{
    public struct UplinkDGram
    { 
        public short m_gram_header;      // 报文头 0xAAA
        public short m_gram_id;          // 报文标识 0xAF1
        public short m_vehicle_id;       // 车辆ID 0x00A ~ 0x00C
        public short m_speed;            // 速度
        public int m_steering;           // 方向盘转角
        public short m_gear;              // 挡位
        public short m_acc_pedal;
        public short m_brake_pedal;
        public short m_rpm;              // RPM 
        public short m_hopper_state;
        public short m_hopper_angle;
        public short m_vehicle_state;    // 0:正常; 1:故障
        public short m_ctrl_sign;        // 1:自动; 2:遥控
        public short m_ctrl_info;        // 控制信息 1:车辆主动请求接管; 2:车辆故障 ...
        public short m_release_feedback;          // 
        public int m_lat_error;
        public int m_lon_error;
        public int m_angle_error;
        public int m_path_x;
        public int m_path_y;
        public short m_path_heading;
        public short m_path_id;
        public short m_basket_filled;
        public short unused0;
        public short m_unuse1;
        public short m_unuse2;
    }

    public struct DownlinkDGram
    {
        public short m_gram_header;      // 报文头 0xAAA
        public short m_gram_id;          // 报文标识 0xAF1  
        public short m_vehicle_id;       // 车辆ID 0x00A ~ 0x00C
        public short m_rc_acc_pedal;
        public short m_rc_brake_pedal;
        public short m_rc_hopper;        // 遥控斗状态，0下降，1上升，2无动作
        public short m_rc_gear;          // 遥控档位 R:-1, N:0, D:1
        public short m_rc_speed;         // 遥控速度
        public short m_ctrl_sign;        // 遥控状态, 1: 遥控; 0: 自动
        public short m_unused;
        public int m_rc_steer;           // 遥控方向盘转角
        public short m_arrive_port;   // 到达装载点信号
        public short m_unuse0;
        public short m_unuse1;
        public short m_unuse2;

       
    }
}
