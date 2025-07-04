using IP2Region.Net.Abstractions;
using IP2Region.Net.XDB;
using Lycoris.Common.Extensions;
using Lycoris.Common.Properties;
using MaxMind.GeoIP2;
using System.Net;
using System.Security.Authentication;

namespace Lycoris.Common.Helper
{
    /// <summary>
    /// IP地址转换工具类
    /// </summary>
    public class IPAddressHelper
    {
        private static ISearcher _ip2RegionSearcher;
        private static DatabaseReader? _maxMindReader;

        /// <summary>
        /// 
        /// </summary>
        public static int IoCount { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        static IPAddressHelper()
        {
            var ip2XdbPath = EmbeddedResourceHelper.ExportToAssemblyScopedPath(Resources.ip2region, "ip2region.xdb");
            _ip2RegionSearcher = new Searcher(CachePolicy.Content, ip2XdbPath);

            var maxMindDbPath = EmbeddedResourceHelper.ExportToAssemblyScopedPath(Resources.maxmind, "maxmind.mmdb");
            _maxMindReader = new DatabaseReader(maxMindDbPath);
        }

        /// <summary>
        /// 加载本地xdb文件
        /// </summary>
        /// <param name="path"></param>
        public static void LoadIP2RegionDb(string path) => _ip2RegionSearcher = new Searcher(CachePolicy.Content, path);

        /// <summary>
        /// 加载本地mmdb文件
        /// </summary>
        /// <param name="path"></param>
        public static void LoadMaxMindDb(string path) => _maxMindReader = new DatabaseReader(path);

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
        /// Uint转IP地址
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
            try
            {
                var address = IPAddress.Parse(ipv4Address);
                var bytes = address.GetAddressBytes();

                return address.AddressFamily switch
                {
                    System.Net.Sockets.AddressFamily.InterNetwork => bytes[0] switch
                    {
                        10 => true,
                        172 when bytes[1] >= 16 && bytes[1] <= 31 => true,
                        192 when bytes[1] == 168 => true,
                        _ => false
                    },
                    System.Net.Sockets.AddressFamily.InterNetworkV6 => address.IsIPv6LinkLocal || address.IsIPv6SiteLocal,
                    _ => false
                };
            }
            catch
            {
                return false;
            }
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

            var result = QueryFromIP2Region(ip);

            if (result.IsPrivate)
                result = QueryFromMaxMind(ipAddress);

            result.IP = ipAddress;

            return result;
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

            var result = QueryFromIP2Region(ip);

            var ipStr = UInt32ToIpv4(ip);

            if (result.IsPrivate)
                result = QueryFromMaxMind(ipStr);

            result.IP = ipStr;

            return result;
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
        /// 获取本机公网IP
        /// </summary>
        /// <returns></returns>
        public static async Task<string?> GetLocalPublicIpAddressAsync()
        {
            var ipaddress = await GetPublicIpAddressByIPIfyAsync();

            if (ipaddress.IsNullOrEmpty())
                ipaddress = await GetPublicIpAddressByICanHazipAsync();

            if (ipaddress.IsNullOrEmpty())
                ipaddress = await GetPublicIpAddressByIfConfigAsync();


            if (ipaddress.IsNullOrEmpty())
                ipaddress = await GetPublicIpAddressByIfConfig2Async();

            if (ipaddress.IsNullOrEmpty())
                ipaddress = await GetPublicIpAddressByIPInfoAsync();

            if (ipaddress.IsNullOrEmpty())
                ipaddress = await GetPublicIpAddressByIPApiAsync();

            return ipaddress.Replace("\n", "").Trim();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        private static IpAttribution QueryFromIP2Region(uint ip)
        {
            var result = _ip2RegionSearcher.Search(ip);

            if (result.IsNullOrEmpty())
                return new IpAttribution() { IsPrivate = true };

            if (result!.Contains("内网IP", StringComparison.CurrentCultureIgnoreCase))
                return new IpAttribution() { IsPrivate = true };

            var addressArray = result!.Split('|');

            return new IpAttribution
            {
                Country = addressArray[0] == "0" ? "" : addressArray[0],
                Area = addressArray[1] == "0" ? "" : addressArray[1],
                Province = addressArray[2] == "0" ? "" : addressArray[2],
                City = addressArray[3] == "0" ? "" : addressArray[3],
                Operator = addressArray[4] == "0" ? "" : addressArray[4]
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        private static IpAttribution QueryFromMaxMind(string ip)
        {
            var attribution = new IpAttribution(ip);

            try
            {
                if (_maxMindReader == null)
                    return attribution;

                var response = _maxMindReader.Country(ip);

                if (response?.Country != null)
                {
                    attribution.Country = response.Country.Names.TryGetValue("zh-CN", out var name)
                        ? name
                        : response.Country.Name;

                    if (response.Continent?.Names?.TryGetValue("zh-CN", out var areaName) == true)
                        attribution.Area = areaName;
                    else
                        attribution.Area = response.Continent?.Name;
                }
            }
            catch { }

            return attribution;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static async Task<string> GetPublicIpAddressByIPIfyAsync()
        {
            var content = await RequestAsync("https://api.ipify.org?format=json");

            // 解析返回的JSON字符串以获取IP地址  
            dynamic ipInfo = Newtonsoft.Json.JsonConvert.DeserializeObject(content!)!;

            var _ipaddress = "";

            if (ipInfo != null)
                _ipaddress = ipInfo.ip;

            return _ipaddress ?? "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static async Task<string> GetPublicIpAddressByICanHazipAsync()
        {
            var content = await RequestAsync("http://icanhazip.com/");
            return content ?? "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static async Task<string> GetPublicIpAddressByIfConfigAsync()
        {
            var content = await RequestAsync("http://ifconfig.me/ip");
            return content ?? "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static async Task<string> GetPublicIpAddressByIfConfig2Async()
        {
            var content = await RequestAsync("http://ifconfig.co/ip");
            return content ?? "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static async Task<string> GetPublicIpAddressByIPInfoAsync()
        {
            var content = await RequestAsync("https://ipinfo.io/ip");
            return content ?? "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static async Task<string> GetPublicIpAddressByIPApiAsync()
        {
            var content = await RequestAsync("http://ip-api.com/json");

            var jobj = content.ToJObject();

            if (!jobj.ContainsKey("status") || jobj["status"]!.ToString() != "success")
                return "";

            if (!jobj.ContainsKey("query"))
                return "";

            return jobj["query"]!.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static async Task<string> RequestAsync(string url)
        {
            var content = "";
            try
            {
                var handler = new HttpClientHandler();
                handler.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13; // 尝试TLS 1.2或TLS 1.3 
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };

                using (var httpClient = new HttpClient(handler))
                {
                    // 这里我们使用一个常见的免费服务来获取公网IP，但请注意这可能会有请求限制或变化  
                    var response = await httpClient.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    content = await response.Content.ReadAsStringAsync();
                }
            }
            catch
            {

            }

            return content ?? "";
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
        public bool IsPrivate { get; set; } = false;

        /// <summary>
        /// 
        /// </summary>
        internal string IP { get; set; } = string.Empty;
    }
}
