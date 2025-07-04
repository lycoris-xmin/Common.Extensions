using System.Net;
using System.Net.Sockets;

namespace Lycoris.Common.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public class ToolHelper
    {
        /// <summary>
        /// 获取网络时间
        /// </summary>
        /// <param name="ntpServer"></param>
        /// <returns></returns>
        public static async Task<DateTime> GetNetworkTimeAsync(string ntpServer = "ntp.ntsc.ac.cn")
        {
            const int NtpPort = 123;
            var ntpData = new byte[48];
            ntpData[0] = 0x1B; // NTP请求头

            var addresses = await Dns.GetHostAddressesAsync(ntpServer);
            var ipEndPoint = new IPEndPoint(addresses[0], NtpPort);

            using var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.ReceiveTimeout = 3000;
            await socket.SendToAsync(ntpData, SocketFlags.None, ipEndPoint);

            var buffer = new ArraySegment<byte>(new byte[48]);
            var receiveResult = await socket.ReceiveAsync(buffer, SocketFlags.None);

            ulong intPart = (ulong)buffer.Array![40] << 24 | (ulong)buffer.Array[41] << 16 | (ulong)buffer.Array[42] << 8 | buffer.Array[43];
            ulong fractPart = (ulong)buffer.Array[44] << 24 | (ulong)buffer.Array[45] << 16 | (ulong)buffer.Array[46] << 8 | buffer.Array[47];

            var milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);

            var networkDateTime = new DateTime(1900, 1, 1).AddMilliseconds((long)milliseconds);

            // 转换到北京时间（UTC+8）
            return networkDateTime.ToLocalTime();
        }
    }
}
