using Lycoris.Common.Extensions;
using Lycoris.Common.Helper;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace Lycoris.Common.Utils.Seo
{
    /// <summary>
    /// SEO工具类
    /// </summary>
    public class SeoUtils
    {
        /// <summary>
        /// 百度SEO推送
        /// </summary>
        /// <param name="token"></param>
        /// <param name="urls"></param>
        /// <returns></returns>
        public async Task<List<string>> BaiduApiPushAsync(string token, List<string> urls)
        {
            if (!urls.HasValue())
                return new List<string>();

            var successList = new List<string>();

            if (token.IsNullOrEmpty())
                return new List<string>();

            var pageIndex = 1;
            var pageSize = 50;

            do
            {
                var list = urls.PageBy(pageIndex, pageSize).ToList();
                if (!list.HasValue())
                    break;

                var apiUrl = $"http://data.zz.baidu.com/urls?site=www.example.com&token={token}";

                var headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } };

                var res = await PushToApiAsync(apiUrl, list, headers);
                if (res.IsNullOrEmpty())
                    break;

                successList.AddRange(list);

                var json = res.ToJObject();
                if (json.TryGetValue("remain", out var remainToken))
                {
                    var remain = remainToken.Value<int>();
                    if (remain == 0)
                        break;

                    if (remain < pageSize)
                        pageSize = remain;
                }

                if (list.Count < pageSize)
                    break;

            } while (true);

            return successList;
        }

        /// <summary>
        /// 必应SEO推送
        /// </summary>
        /// <param name="akpiKey"></param>
        /// <param name="urls"></param>
        /// <returns></returns>
        public async Task<List<string>> BingApiPushAsync(string akpiKey, List<string> urls)
        {
            if (!urls.HasValue())
                return new List<string>();

            if (akpiKey.IsNullOrEmpty())
                return new List<string>();

            var successList = new List<string>();

            var pageIndex = 1;
            var pageSize = 50;

            do
            {
                var list = urls.PageBy(pageIndex, pageSize).ToList();
                if (!list.HasValue())
                    break;

                var apiUrl = $"https://ssl.bing.com/webmaster/api.svc/json/SubmitUrlbatch?apikey={akpiKey}";

                var headers = new Dictionary<string, string> { { "Content-Type", "application/json" } };

                var payload = new { siteUrl = "https://www.example.com", urlList = list };

                var res = await PushToApiAsync(apiUrl, new List<string>() { payload.ToJson() }, headers);
                if (res.IsNullOrEmpty())
                    break;

                successList.AddRange(list);

                if (list.Count < pageSize)
                    break;

            } while (true);

            return successList;
        }

        /// <summary>
        /// 谷歌SEO推送
        /// </summary>
        /// <param name="akpiKey"></param>
        /// <param name="urls"></param>
        /// <returns></returns>
        public async Task<List<string>> GoogleApiPushAsync(string akpiKey, List<string> urls)
        {
            if (!urls.HasValue())
                return new List<string>();

            if (akpiKey.IsNullOrEmpty())
                return new List<string>();

            var successList = new List<string>();

            var pageIndex = 1;
            var pageSize = 50;
            do
            {
                var list = urls.PageBy(pageIndex, pageSize).ToList();
                if (!list.HasValue())
                    break;

                var apiUrl = $"https://indexing.googleapis.com/v3/urlNotifications:publish?key={akpiKey}";

                var headers = new Dictionary<string, string> { { "Content-Type", "application/json" } };

                var payload = new { urls = list };

                var res = await PushToApiAsync(apiUrl, new List<string>() { payload.ToJson() }, headers);

                if (res.IsNullOrEmpty())
                    break;

                successList.AddRange(list);

                var json = res.ToJObject();
                if (json.TryGetValue("dailyLimit", out var remainToken))
                {
                    var dailyLimit = remainToken.Value<int>();
                    if (dailyLimit == 0)
                        break;

                    if (dailyLimit < pageSize)
                        pageSize = dailyLimit;
                }

                if (list.Count < pageSize)
                    break;

            } while (true);

            return successList;
        }

        /// <summary>
        /// 百度失效链接推送
        /// </summary>
        /// <param name="token"></param>
        /// <param name="urls"></param>
        /// <returns></returns>
        public async Task NotifyBaiduUrlInvalidAsync(string token, List<string> urls)
        {
            if (token.IsNullOrEmpty())
                return;

            var apiUrl = $"http://data.zz.baidu.com/del?site=www.example.com&token={token}";

            var headers = new Dictionary<string, string> { { "Content-Type", "text/plain" } };

            await PushToApiAsync(apiUrl, urls, headers);
        }

        /// <summary>
        /// 谷歌失效链接推送
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task NotifyGoogleUrlInvalidAsync(string apiKey, string url)
        {
            if (apiKey.IsNullOrEmpty())
                return;

            var apiUrl = $"https://indexing.googleapis.com/v3/urlNotifications:publish?key={apiKey}";

            var headers = new Dictionary<string, string> { { "Content-Type", "application/json" } };

            var payload = new
            {
                url = url,
                type = "URL_DELETED" // 通知 URL 已失效
            };

            await PushToApiAsync(apiUrl, new List<string> { payload.ToJson() }, headers);
        }

        /// <summary>
        /// 生成 Sitemap 文件
        /// </summary>
        /// <param name="urls"></param>
        /// <param name="savePath"></param>
        /// <returns></returns>
        public List<string> GenerateSitemapFiles(List<string> urls, string savePath)
        {
            FileHelper.EnsurePathExists(savePath);

            var sitemapFiles = new List<string>();
            int fileIndex = 1;
            for (int i = 0; i < urls.Count; i += 500)
            {
                var sitemapUrls = urls.GetRange(i, Math.Min(500, urls.Count - i));
                string sitemapPath = Path.Combine(savePath, $"sitemap-{fileIndex}.xml");

                using (var writer = new StreamWriter(sitemapPath))
                {
                    writer.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
                    writer.WriteLine("<urlset xmlns=\"http://www.sitemaps.org/schemas/sitemap/0.9\">");

                    foreach (var url in sitemapUrls)
                    {
                        writer.WriteLine("  <url>");
                        writer.WriteLine($"    <loc>{url}</loc>");
                        writer.WriteLine("  </url>");
                    }

                    writer.WriteLine("</urlset>");
                }

                sitemapFiles.Add(sitemapPath);
                fileIndex++;
            }

            return sitemapFiles;
        }

        /// <summary>
        /// 推送 Sitemap 到百度
        /// </summary>
        /// <param name="token"></param>
        /// <param name="sitemapUrls"></param>
        /// <returns></returns>
        public async Task PushSitemapToBaiduAsync(string token, List<string> sitemapUrls)
        {
            if (token.IsNullOrEmpty())
                return;

            var apiUrl = $"http://data.zz.baidu.com/ping?sitemap={string.Join(",", sitemapUrls)}&token={token}";

            await PushToApiAsync(apiUrl, new List<string>(), null);
        }

        /// <summary>
        /// 推送 Sitemap 到谷歌
        /// </summary>
        /// <param name="sitemapUrls"></param>
        /// <returns></returns>
        public async Task PushSitemapToGoogleAsync(List<string> sitemapUrls)
        {
            foreach (var sitemapUrl in sitemapUrls)
            {
                string apiUrl = $"https://www.google.com/ping?sitemap={sitemapUrl}";
                await PushToApiAsync(apiUrl, new List<string>(), null);
            }
        }

        /// <summary>
        /// 通用的 API 推送方法
        /// </summary>
        /// <param name="apiUrl"></param>
        /// <param name="urls"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        private async Task<string?> PushToApiAsync(string apiUrl, List<string> urls, Dictionary<string, string>? headers = null)
        {
            if (string.IsNullOrEmpty(apiUrl))
                throw new ArgumentException("API URL 不能为空", nameof(apiUrl));

            if (urls == null || urls.Count == 0)
                throw new ArgumentException("推送的 URL 列表不能为空", nameof(urls));

            // 初始化 RestClient
            var client = new RestClient();

            // 设置请求
            var request = new RestRequest(apiUrl, Method.Post);

            // 添加请求头
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.AddHeader(header.Key, header.Value);
                }
            }

            // 设置请求体
            string payload = string.Join("\n", urls);
            request.AddParameter("text/plain", payload, ParameterType.RequestBody);

            // 发送请求
            var response = await client.ExecuteAsync(request);

            // 返回结果
            return response.IsSuccessful ? response.Content : "";
        }
    }
}
