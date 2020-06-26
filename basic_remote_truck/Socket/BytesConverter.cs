using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace RCComm
{
    class BytesConverter
    {
        /// <summary>
        /// 将结构体转换成Byte数组，用于Socket发送
        /// </summary>
        /// <typeparam name="T">[in type] 结构体类型</typeparam>
        /// <param name="_struct">[in] 结构体对象</param>
        /// <returns>转换后的Byte数组</returns>
        public static byte[] StructToBytes<T>(T _struct)
        {
            int size = Marshal.SizeOf(typeof(T));
            byte[] arr_byte = new byte[size];
            IntPtr ptr_buffer = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(_struct, ptr_buffer, true);
                Marshal.Copy(ptr_buffer, arr_byte, 0, size);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr_buffer);
            }
            return arr_byte;
        }

        /// <summary>
        /// 将Byte数组转换成特定类型T的结构体，用于Socket接收
        /// </summary>
        /// <typeparam name="T">[return type] 结构体类型</typeparam>
        /// <param name="_arr_byte">[in] 将转换成结构体T的Byte数组</param>
        /// <returns>结构体T对象</returns>
        public static T BytesToStruct<T>(byte[] _arr_byte)
        {
            object ret_obj = null;
            int size = Marshal.SizeOf(typeof(T));
            IntPtr ptr_buffer = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(_arr_byte, 0, ptr_buffer, size);
                ret_obj = Marshal.PtrToStructure(ptr_buffer, typeof(T));
            }
            finally
            {
                Marshal.FreeHGlobal(ptr_buffer);
            }
            return (T)ret_obj;
        }
    }
}
