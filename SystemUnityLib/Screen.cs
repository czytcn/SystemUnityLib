using System;
using System.Runtime.InteropServices;

namespace SystemUnityLib
{
    /// <summary>
    /// 屏幕类
    /// </summary>
    public class Screen
    {
        [DllImport("gdi32.dll")]
        internal static extern bool SetDeviceGammaRamp(IntPtr hDC, ref RAMP lpRamp);
        [DllImport("gdi32.dll")]
        internal static extern int GetDeviceGammaRamp(IntPtr hDC, ref RAMP lpRamp);
        [DllImport("user32.dll")]
        internal static extern IntPtr GetDC(IntPtr hWnd);
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]

        public struct RAMP
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public UInt16[] Red;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public UInt16[] Green;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
            public UInt16[] Blue;
        }
        /// <summary>
        /// 设置伽马值（亮度）
        /// </summary>
        /// <param name="gamma">小于等于256</param>
        /// <returns></returns>
        public bool SetGamma(int gamma)
        {
            if (gamma <= 256 && gamma >= 1)
            {
                RAMP ramp = new RAMP();
                ramp.Red = new ushort[256];
                ramp.Green = new ushort[256];
                ramp.Blue = new ushort[256];
                for (int i = 1; i < 256; i++)
                {
                    int iArrayValue = i * (gamma + 128);

                    if (iArrayValue > 65535)
                        iArrayValue = 65535;
                    ramp.Red[i] = ramp.Blue[i] = ramp.Green[i] = (ushort)iArrayValue;
                }
                return SetDeviceGammaRamp(GetDC(IntPtr.Zero), ref ramp);
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 返回伽马值
        /// </summary>
        /// <returns></returns>
        public RAMP GetGamma()
        {
            RAMP r = new RAMP();
            GetDeviceGammaRamp(GetDC(IntPtr.Zero), ref r);
            return r;
        }

    }
}
