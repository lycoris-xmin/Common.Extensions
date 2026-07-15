using IP2Region.Net.Abstractions;
using IP2Region.Net.XDB;
using Lycoris.Common.Extensions;
using Lycoris.Common.Properties;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Security.Authentication;

namespace Lycoris.Common.Helper
{
    /// <summary>
    /// IP 地址工具类
    /// </summary>
    public class IPAddressHelper
    {
        private static ISearcher _ip2RegionV4 = null!;
        private static ISearcher _ip2RegionV6 = null!;
        private static readonly HttpClient _httpClient;

        /// <summary>
        /// 总查询次数
        /// </summary>
        public static int IoCount => (_ip2RegionV4?.IoCount ?? 0) + (_ip2RegionV6?.IoCount ?? 0);

        static IPAddressHelper()
        {
            var v4Path = EmbeddedResourceHelper.ExportToAssemblyScopedPath(Resources.ip2region_v4, "ip2region_v4.xdb");
            _ip2RegionV4 = new Searcher(CachePolicy.Content, v4Path);

            var v6Path = EmbeddedResourceHelper.ExportToAssemblyScopedPath(Resources.ip2region_v6, "ip2region_v6.xdb");
            _ip2RegionV6 = new Searcher(CachePolicy.Content, v6Path);

            var handler = new HttpClientHandler
            {
                SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13,
                ServerCertificateCustomValidationCallback = (_, _, _, _) => true
            };
            _httpClient = new HttpClient(handler);
        }

        /// <summary>
        /// 加载本地 IPv4 xdb 文件
        /// </summary>
        /// <param name="path">xdb 文件路径</param>
        public static void LoadIP2RegionV4Db(string path)
        {
            if (_ip2RegionV4 is IDisposable disposable)
                disposable.Dispose();
            _ip2RegionV4 = new Searcher(CachePolicy.Content, path);
        }

        /// <summary>
        /// 加载本地 IPv6 xdb 文件
        /// </summary>
        /// <param name="path">xdb 文件路径</param>
        public static void LoadIP2RegionV6Db(string path)
        {
            if (_ip2RegionV6 is IDisposable disposable)
                disposable.Dispose();
            _ip2RegionV6 = new Searcher(CachePolicy.Content, path);
        }

        /// <summary>
        /// IP 地址转 UInt32
        /// </summary>
        /// <param name="ipAddress">IP 地址字符串</param>
        /// <returns>UInt32</returns>
        public static uint Ipv4ToUInt32(string ipAddress)
        {
            if (string.IsNullOrEmpty(ipAddress))
                return 0u;

            return Ipv4ToUInt32(IPAddress.Parse(ipAddress));
        }

        /// <summary>
        /// IP 地址转 UInt32
        /// </summary>
        /// <param name="ipAddress">IP 地址</param>
        /// <returns>UInt32</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint Ipv4ToUInt32(IPAddress ipAddress)
        {
            var bytes = ipAddress.GetAddressBytes();
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return BitConverter.ToUInt32(bytes, 0);
        }

        /// <summary>
        /// UInt32 转 IP 地址字符串
        /// </summary>
        /// <param name="ipAddressUint">UInt32 值</param>
        /// <returns>IP 地址字符串</returns>
        public static string UInt32ToIpv4(uint? ipAddressUint)
        {
            if (!ipAddressUint.HasValue || ipAddressUint.GetValueOrDefault() == 0)
                return "";

            return UInt32ToIpv4(ipAddressUint.Value);
        }

        /// <summary>
        /// UInt32 转 IP 地址字符串
        /// </summary>
        /// <param name="ipAddressUint">UInt32 值</param>
        /// <returns>IP 地址字符串</returns>
        public static string UInt32ToIpv4(uint ipAddressUint)
        {
            var bytes = BitConverter.GetBytes(ipAddressUint);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return new IPAddress(bytes).ToString();
        }

        /// <summary>
        /// IP 地址字符串转 IPAddress
        /// </summary>
        /// <param name="ipAddress">IP 地址字符串</param>
        /// <returns>IPAddress</returns>
        public static IPAddress? Ipv4ToIpAddress(string ipAddress)
        {
            if (ipAddress.IsNullOrEmpty())
                return null;

            return IPAddress.Parse(ipAddress);
        }

        /// <summary>
        /// UInt32 转 IPAddress
        /// </summary>
        /// <param name="ipAddress">UInt32 值</param>
        /// <returns>IPAddress</returns>
        public static IPAddress Ipv4ToIpAddress(uint ipAddress)
        {
            var bytes = BitConverter.GetBytes(ipAddress);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);
            return new IPAddress(bytes);
        }

        /// <summary>
        /// 是否局域网 IP
        /// </summary>
        /// <param name="ipv4Address">IP 地址字符串</param>
        /// <returns>是否局域网</returns>
        public static bool IsPrivateNetwork(string ipv4Address)
        {
            try
            {
                return IsPrivateNetwork(IPAddress.Parse(ipv4Address));
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 是否局域网 IP
        /// </summary>
        /// <param name="address">IP 地址</param>
        /// <returns>是否局域网</returns>
        public static bool IsPrivateNetwork(IPAddress address)
        {
            var bytes = address.GetAddressBytes();
            return address.AddressFamily switch
            {
                AddressFamily.InterNetwork => bytes[0] switch
                {
                    10 => true,
                    127 => true,
                    172 when bytes[1] >= 16 && bytes[1] <= 31 => true,
                    192 when bytes[1] == 168 => true,
                    _ => false
                },
                AddressFamily.InterNetworkV6 => address.IsIPv6LinkLocal || address.IsIPv6SiteLocal,
                _ => false
            };
        }

        /// <summary>
        /// 查询 IP 归属地（自动判断内网/公网、IPv4/IPv6）
        /// </summary>
        /// <param name="ipAddress">IP 地址字符串</param>
        /// <returns>归属地信息</returns>
        public static IpAttribution Search(string ipAddress)
        {
            if (ipAddress.IsNullOrEmpty())
                return new IpAttribution(ipAddress ?? "");

            if (!IPAddress.TryParse(ipAddress, out var address))
                return new IpAttribution(ipAddress);

            return Search(address);
        }

        /// <summary>
        /// 查询 IP 归属地（自动判断内网/公网、IPv4/IPv6）
        /// </summary>
        /// <param name="ipAddress">IP 地址</param>
        /// <returns>归属地信息</returns>
        public static IpAttribution Search(IPAddress ipAddress)
        {
            if (IsPrivateNetwork(ipAddress))
                return new IpAttribution(ipAddress.ToString()) { IsPrivate = true };

            return SearchByFamily(ipAddress);
        }

        /// <summary>
        /// 根据 IP 版本选择对应的 searcher 进行查询
        /// </summary>
        private static IpAttribution SearchByFamily(IPAddress address)
        {
            try
            {
                var searcher = address.AddressFamily == AddressFamily.InterNetworkV6 ? _ip2RegionV6 : _ip2RegionV4;
                var result = searcher.Search(address);

                var ipStr = address.ToString();

                if (string.IsNullOrEmpty(result))
                    return new IpAttribution(ipStr);

                // region format: Country|Province|City|ISP|CountryCode
                var parts = result.Split('|');

                return new IpAttribution(ipStr)
                {
                    Country = parts.Length > 0 && parts[0] != "0" ? parts[0] : null,
                    Province = parts.Length > 1 && parts[1] != "0" ? parts[1] : null,
                    City = parts.Length > 2 && parts[2] != "0" ? parts[2] : null,
                    Operator = parts.Length > 3 && parts[3] != "0" ? parts[3] : null,
                };
            }
            catch
            {
                return new IpAttribution(address.ToString());
            }
        }

        /// <summary>
        /// 查询结果地址转换
        /// </summary>
        /// <param name="input">归属地信息</param>
        /// <param name="municipality">直辖市列表</param>
        /// <returns>格式化地址</returns>
        public static string ChangeAddress(IpAttribution input, params string[]? municipality)
        {
            if (input.IsPrivate)
                return "局域网";

            municipality ??= ["北京市", "上海市", "重庆市", "深圳市"];

            try
            {
                if (string.IsNullOrEmpty(input.Country))
                    return "未知";

                if (input.Country == "中国")
                {
                    if (!input.Province.IsNullOrEmpty() && municipality.Contains(input.Province))
                        return input.Province!;
                    if (!input.City.IsNullOrEmpty() && municipality.Contains(input.City))
                        return input.City!;
                    return $"{input.Province ?? ""} {input.City ?? ""}".Trim();
                }

                if (!string.IsNullOrEmpty(input.Province))
                    return $"{input.Country} {input.Province}";
                return !string.IsNullOrEmpty(input.City) ? $"{input.Country} {input.City}" : input.Country;
            }
            catch
            {
                return "未知";
            }
        }

        /// <summary>
        /// 获取本机公网 IP
        /// </summary>
        /// <returns>公网 IP 地址</returns>
        public static async Task<string?> GetLocalPublicIpAddressAsync()
        {
            var providers = new Func<Task<string?>>[]
            {
                GetPublicIpAddressByIPIfyAsync,
                GetPublicIpAddressByICanHazipAsync,
                GetPublicIpAddressByIfConfigAsync,
                GetPublicIpAddressByIfConfig2Async,
                GetPublicIpAddressByIPInfoAsync,
                GetPublicIpAddressByIPApiAsync,
            };

            foreach (var provider in providers)
            {
                var ipaddress = await provider().ConfigureAwait(false);
                if (!string.IsNullOrEmpty(ipaddress))
                    return ipaddress.Replace("\n", "").Trim();
            }

            return null;
        }

        private static async Task<string?> GetPublicIpAddressByIPIfyAsync()
        {
            var content = await RequestAsync("https://api.ipify.org?format=json");
            if (string.IsNullOrEmpty(content))
                return "";

            try
            {
                dynamic? ipInfo = Newtonsoft.Json.JsonConvert.DeserializeObject(content);
                return ipInfo?.ip ?? "";
            }
            catch
            {
                return "";
            }
        }

        private static async Task<string?> GetPublicIpAddressByICanHazipAsync()
        {
            return await RequestAsync("http://icanhazip.com/") ?? "";
        }

        private static async Task<string?> GetPublicIpAddressByIfConfigAsync()
        {
            return await RequestAsync("http://ifconfig.me/ip") ?? "";
        }

        private static async Task<string?> GetPublicIpAddressByIfConfig2Async()
        {
            return await RequestAsync("http://ifconfig.co/ip") ?? "";
        }

        private static async Task<string?> GetPublicIpAddressByIPInfoAsync()
        {
            return await RequestAsync("https://ipinfo.io/ip") ?? "";
        }

        private static async Task<string?> GetPublicIpAddressByIPApiAsync()
        {
            var content = await RequestAsync("http://ip-api.com/json");
            if (string.IsNullOrEmpty(content))
                return "";

            var jobj = content.ToJObject();

            if (jobj["status"]?.ToString() != "success")
                return "";

            return jobj["query"]?.ToString() ?? "";
        }

        private static async Task<string> RequestAsync(string url)
        {
            try
            {
                var response = await _httpClient.GetAsync(url).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
            catch
            {
                return "";
            }
        }
    }

    /// <summary>
    /// IP 归属地信息
    /// </summary>
    public class IpAttribution
    {
        /// <summary>
        /// </summary>
        public IpAttribution() { }

        /// <summary>
        /// </summary>
        /// <param name="ip">IP 地址</param>
        public IpAttribution(string ip) { IP = ip; }

        /// <summary>
        /// 国家
        /// </summary>
        public string? Country { get; set; }

        /// <summary>
        /// 大区（国外洲名等）
        /// </summary>
        public string? Area { get; set; }

        /// <summary>
        /// 省份
        /// </summary>
        public string? Province { get; set; }

        /// <summary>
        /// 城市
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

        internal string IP { get; set; } = string.Empty;
    }
}
