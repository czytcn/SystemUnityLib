using System;
using System.Runtime.InteropServices;

namespace SystemUnityLib
{
    /// <summary>
    /// 检测启动模式的静态类
    /// Detect Boot Mode Is Bios Or UEFI
    /// </summary>
    public static class BootMode
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern UInt32 GetFirmwareEnvironmentVariableA([MarshalAs(UnmanagedType.LPWStr)]string lpName, [MarshalAs(UnmanagedType.LPWStr)] string lpGuid, IntPtr pBuffer, UInt32 nSize);


        /// <summary>
        /// 返回是否是UEFI模式启动
        /// </summary>
        /// <returns></returns>
        public static bool IsUEFIMode()
        {
            //ERROR_INVALID_FUNCTION 1(0x1)
            //ERROR_NOACCESS 998(0x3E6)
            GetFirmwareEnvironmentVariableA("", "{00000000-0000-0000-0000-000000000000}", IntPtr.Zero, 0);
            if (Marshal.GetLastWin32Error() == 0x1)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

    }

}
