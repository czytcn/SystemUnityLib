﻿using System;
using System.Net;
using System.Net.Sockets;

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
            return networkDateTime.ToLocalTime();
        }

    }
}
