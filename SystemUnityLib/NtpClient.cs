using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace SystemUnityLib
{
    /// <summary>
    /// Ntp客户端
    ///  Ntp Client
    /// </summary>
    public class NtpClient
    {
        /// <summary>
        /// NTP server服务器
        /// </summary>

        public string NtpServer { get; set; } = "cn.ntp.org.cn";

        public NtpClient(string NtpServer)
        {
            this.NtpServer = NtpServer;
        }
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool SetSystemTime(ref SYSTEMTIME systime);
        /// <summary>
        /// 设置本机时间
        /// </summary>
        /// <param name="datetime"></param>
        /// <returns></returns>
        public bool SetLocalMachineDateTime(DateTime datetime)
        {
            SYSTEMTIME time = new SYSTEMTIME();
            time.wYear = (short)datetime.Year;
            time.wMonth = (short)datetime.Month;
            time.wDay = (short)datetime.Day;
            time.wHour = (short)datetime.Hour;
            time.wMinute = (short)datetime.Minute;
            time.wSecond = (short)datetime.Second;
            return SetSystemTime(ref time);
        }
        /// <summary>
        /// 从NtpServer获取时间
        /// </summary>
        /// <returns></returns>
        public DateTime GetTimeFromNtpServer()
        {
            string ntpServer = this.NtpServer;
            var ntpData = new byte[48];
            ntpData[0] = 0x1B; //LeapIndicator = 0 (no warning), VersionNum = 3  (IPv4 only), Mode = 3 (Client Mode)
            var addresses = Dns.GetHostEntry(ntpServer).AddressList;
            var ipEndPoint = new IPEndPoint(addresses[0], 123);
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
            {
                socket.SendTimeout = 3000;
                try
                {
                    socket.Connect(ipEndPoint);
                    socket.Send(ntpData);
                    socket.Receive(ntpData);
                }
                catch (Exception)
                {
                }
            }
            ulong intPart = (ulong)ntpData[40] << 24 | (ulong)ntpData[41] << 16 | (ulong)ntpData[42] << 8 | ntpData[43];
            ulong fractPart = (ulong)ntpData[44] << 24 | (ulong)ntpData[45] << 16 | (ulong)ntpData[46] << 8 | ntpData[47];
            var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);
            var networkDateTime = (new DateTime(1900, 1, 1)).AddMilliseconds((long)milliseconds);
            return networkDateTime;
        }

        /// <summary>
        /// 时间结构体
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        internal struct SYSTEMTIME
        {
            public short wYear;
            public short wMonth;
            public short wDayOfWeek;
            public short wDay;
            public short wHour;
            public short wMinute;
            public short wSecond;
            public short wMilliseconds;
        }

    }
}
