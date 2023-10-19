using Lycoris.Common.Extensions;
using Lycoris.Common.Properties;
using System.Net;
using System.Text;

namespace Lycoris.Common.Helper
{
    /// <summary>
    /// IP地址转换工具类
    /// </summary>
    public class IPAddressHelper
    {
        private const int HeaderInfoLength = 256;
        private const int VectorIndexRows = 256;
        private const int VectorIndexCols = 256;
        private const int VectorIndexSize = 8;
        private const int SegmentIndexSize = 14;

        private static readonly byte[]? _vectorIndex;
        private static readonly MemoryStream? _contentStream;
        private static byte[]? _contentBuff;
        private static FileStream? _fileStream;
        private static CachePolicy _cachePolicy = CachePolicy.Content;

        /// <summary>
        /// 
        /// </summary>
        public static int IoCount { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        static IPAddressHelper()
        {
            _contentStream = new MemoryStream(Resources.ip2region);

            switch (_cachePolicy)
            {
                case CachePolicy.Content:
                    using (var stream = new MemoryStream())
                    {
                        _contentStream.CopyTo(stream);
                        _contentBuff = stream.ToArray();
                    }
                    break;
                case CachePolicy.VectorIndex:
                    var vectorLength = VectorIndexRows * VectorIndexCols * VectorIndexSize;
                    _vectorIndex = new byte[vectorLength];
                    Read(HeaderInfoLength, _vectorIndex);
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        ~IPAddressHelper()
        {
            if (_contentStream != null)
            {
                _contentStream.Close();
                _contentStream.Dispose();
            }

            if (_fileStream != null)
            {
                _fileStream.Close();
                _fileStream.Dispose();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <exception cref="FileNotFoundException"></exception>
        public static void LoadXDBFile(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException(filePath);

            _fileStream = File.OpenRead(filePath);

            if (_contentStream != null)
            {
                _contentStream.Close();
                _contentStream.Dispose();
            }

            using var stream = new MemoryStream();
            _fileStream.CopyTo(stream);
            _contentBuff = stream.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static uint GetMidIp(uint x, uint y) => (x & y) + ((x ^ y) >> 1);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static int GetMidIp(int x, int y) => (x & y) + ((x ^ y) >> 1);

        /// <summary>
        /// IP地址转Uint
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public static uint Ipv4ToUInt32(string ipAddress)
        {
            if (string.IsNullOrEmpty(ipAddress))
                return 0u;

            var address = IPAddress.Parse(ipAddress);
            return Ipv4ToUInt32(address);
        }

        /// <summary>
        /// IP地址转Uint
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public static uint Ipv4ToUInt32(IPAddress ipAddress)
        {
            try
            {
                var bytes = ipAddress.GetAddressBytes();
                Array.Reverse(bytes);
                return BitConverter.ToUInt32(bytes, 0);
            }
            catch
            {
                return 0u;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipAddressUint"></param>
        /// <returns></returns>
        public static string UInt32ToIpv4(uint? ipAddressUint)
        {
            if (!ipAddressUint.HasValue || ipAddressUint.Value == 0)
                return "";

            try
            {
                byte[] bytes = BitConverter.GetBytes(ipAddressUint.Value);
                Array.Reverse(bytes);
                var ipAddress = new IPAddress(bytes);
                return ipAddress.ToString();
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// Uint转IP地址
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public static IPAddress? Ipv4ToIpAddress(string ipAddress)
        {
            if (ipAddress.IsNullOrEmpty())
                return null;

            return IPAddress.Parse(ipAddress);
        }

        /// <summary>
        /// Uint转IP地址
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns></returns>
        public static IPAddress Ipv4ToIpAddress(uint ipAddress)
        {
            var bytes = BitConverter.GetBytes(ipAddress);
            Array.Reverse(bytes);
            return new IPAddress(bytes);
        }

        /// <summary>
        /// 是否局域网IP
        /// </summary>
        /// <param name="ipv4Address"></param>
        /// <returns></returns>
        public static bool IsPrivateNetwork(string ipv4Address)
        {
            if (ipv4Address.ToLower() == "localhost" || ipv4Address == "127.0.0.1")
                return true;

            if (IPAddress.TryParse(ipv4Address, out _))
            {
                if (ipv4Address.StartsWith("192.168.") || ipv4Address.StartsWith("10."))
                    return true;

                if (ipv4Address.StartsWith("172."))
                {
                    string seg2 = ipv4Address[4..7];
                    if (seg2.EndsWith('.') && string.Compare(seg2, "16.") >= 0 && string.Compare(seg2, "31.") <= 0)
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 查询IP归属地
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns>国家|区域|省份|城市|运营商</returns>
        public static IpAttribution Search(string ipAddress)
        {
            if (IsPrivateNetwork(ipAddress))
                return new IpAttribution(ipAddress) { IsPrivate = true };

            var ip = Ipv4ToUInt32(ipAddress);
            var ipStr = Search(ip);

            return ChangeIpAttribution(ipStr);
        }

        /// <summary>
        /// 查询IP归属地
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <returns>国家|区域|省份|城市|运营商</returns>
        public static IpAttribution Search(IPAddress ipAddress)
        {
            if (IsPrivateNetwork(ipAddress.ToString()))
                return new IpAttribution(ipAddress.ToString()) { IsPrivate = true };

            var ip = Ipv4ToUInt32(ipAddress);
            var ipStr = Search(ip);
            if (ipStr.IsNullOrEmpty())
                return new IpAttribution(ipAddress.ToString());

            return ChangeIpAttribution(ipStr);
        }

        /// <summary>
        /// 查询结果转换
        /// </summary>
        /// <param name="input"></param>
        /// <param name="Municipality"></param>
        /// <returns></returns>
        public static string ChangeAddress(IpAttribution input, params string[]? Municipality)
        {
            if (input.IsPrivate)
                return "局域网";

            Municipality ??= new string[] { "北京市", "上海市", "重庆市", "深圳市" };

            try
            {
                if (string.IsNullOrEmpty(input.Country))
                    return "未知";

                if (input.Country == "中国")
                {
                    if (!input.Province.IsNullOrEmpty() && Municipality.Contains(input.Province))
                        return input.Province!;
                    else if (!input.City.IsNullOrEmpty() && Municipality.Contains(input.City))
                        return input.City!;
                    else
                        return $"{input.Province ?? ""} {input.City ?? ""}".Trim();
                }

                if (!string.IsNullOrEmpty(input.Area))
                    return $"{input.Country} {input.Area}";
                else if (!string.IsNullOrEmpty(input.Province))
                    return $"{input.Country} {input.Province}";
                else if (!string.IsNullOrEmpty(input.City))
                    return $"{input.Country} {input.City}";
                else
                    return input.Country;
            }
            catch
            {
                return "未知";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <returns>国家|区域|省份|城市|运营商</returns>
        private static string? Search(uint ip)
        {
            var il0 = ip >> 24 & 0xFF;
            var il1 = ip >> 16 & 0xFF;
            var idx = il0 * VectorIndexCols * VectorIndexSize + il1 * VectorIndexSize;

            uint sPtr = 0, ePtr = 0;

            switch (_cachePolicy)
            {
                case CachePolicy.VectorIndex:
                    {
                        sPtr = BitConverter.ToUInt32(_vectorIndex.AsSpan()[(int)idx..]);
                        ePtr = BitConverter.ToUInt32(_vectorIndex.AsSpan()[((int)idx + 4)..]);
                    }
                    break;
                case CachePolicy.Content:
                    {
                        sPtr = BitConverter.ToUInt32(_contentBuff.AsSpan()[(HeaderInfoLength + (int)idx)..]);
                        ePtr = BitConverter.ToUInt32(_contentBuff.AsSpan()[(HeaderInfoLength + (int)idx + 4)..]);
                    }
                    break;
                case CachePolicy.File:
                    {
                        var buff = new byte[VectorIndexSize];
                        Read((int)(idx + HeaderInfoLength), buff);
                        sPtr = BitConverter.ToUInt32(buff);
                        ePtr = BitConverter.ToUInt32(buff.AsSpan()[4..]);
                    }
                    break;
            }


            var dataLen = 0;
            uint dataPtr = 0;
            var l = 0;
            var h = (int)((ePtr - sPtr) / SegmentIndexSize);
            var buffer = new byte[SegmentIndexSize];

            while (l <= h)
            {
                var mid = GetMidIp(l, h);
                var pos = sPtr + mid * SegmentIndexSize;

                Read((int)pos, buffer);
                var sip = BitConverter.ToUInt32(buffer);

                if (ip < sip)
                    h = mid - 1;
                else
                {
                    var eip = BitConverter.ToUInt32(buffer.AsSpan()[4..]);
                    if (ip > eip)
                        l = mid + 1;
                    else
                    {
                        dataLen = BitConverter.ToUInt16(buffer.AsSpan()[8..]);
                        dataPtr = BitConverter.ToUInt32(buffer.AsSpan()[10..]);
                        break;
                    }
                }
            }

            if (dataLen == 0)
                return default;

            var regionBuff = new byte[dataLen];
            Read((int)dataPtr, regionBuff);

            return Encoding.UTF8.GetString(regionBuff);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="buff"></param>
        /// <exception cref="IOException"></exception>
        private static void Read(int offset, byte[] buff)
        {
            switch (_cachePolicy)
            {
                case CachePolicy.Content:
                    _contentBuff.AsSpan()[offset..(offset + buff.Length)].CopyTo(buff);
                    break;
                default:
                    {
                        _contentStream!.Seek(offset, SeekOrigin.Begin);
                        IoCount++;

                        var rLen = _contentStream.Read(buff);
                        if (rLen != buff.Length)
                            throw new IOException($"incomplete read: readed bytes should be {buff.Length}");
                    }
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ipStr"></param>
        /// <returns></returns>
        private static IpAttribution ChangeIpAttribution(string? ipStr)
        {
            if (ipStr.IsNullOrEmpty())
                return new IpAttribution() { IsPrivate = true };

            if (ipStr!.Contains("内网IP", StringComparison.CurrentCultureIgnoreCase))
                return new IpAttribution() { IsPrivate = true };

            var addressArray = ipStr!.Split('|');

            return new IpAttribution
            {
                Country = addressArray[0] == "0" ? "" : addressArray[0],
                Area = addressArray[1] == "0" ? "" : addressArray[1],
                Province = addressArray[2] == "0" ? "" : addressArray[2],
                City = addressArray[3] == "0" ? "" : addressArray[3],
                Operator = addressArray[4] == "0" ? "" : addressArray[4]
            };
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class IpAttribution
    {
        /// <summary>
        /// 
        /// </summary>
        public IpAttribution()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="IP"></param>
        public IpAttribution(string IP)
        {
            this.IP = IP;
        }

        /// <summary>
        /// 国家
        /// </summary>
        public string? Country { get; set; }

        /// <summary>
        /// 大区(国外国家的洲名等)
        /// </summary>
        public string? Area { get; set; }

        /// <summary>
        /// 省份
        /// </summary>
        public string? Province { get; set; }

        /// <summary>
        /// 室
        /// </summary>
        public string? City { get; set; }

        /// <summary>
        /// 运营商
        /// </summary>
        public string? Operator { get; set; }

        /// <summary>
        /// 是否局域网
        /// </summary>
        public bool IsPrivate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        internal string IP { get; set; } = string.Empty;
    }

    internal enum CachePolicy
    {
        /// <summary>
        /// no cache , not thread safe!
        /// </summary>
        File,
        /// <summary>
        /// cache vector index , reduce the number of IO operations , not thread safe!
        /// </summary>
        VectorIndex,
        /// <summary>
        /// default cache policy , cache whole xdb file , thread safe 
        /// </summary>
        Content
    }
}
