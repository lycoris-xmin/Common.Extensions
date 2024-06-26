using Lycoris.Common.Extensions;
using Lycoris.Common.Http.Options;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace Lycoris.Common.Http
{
    /// <summary>
    /// 
    /// </summary>
    public class HttpUtils
    {
        /// <summary>
        /// 
        /// </summary>
        private const string _DefaultUserAgent = "HttpClient";

        /// <summary>
        /// 
        /// </summary>
        private readonly string[] ContentTypeKey = new string[] { "contetntype", "contetn-type" };

        /// <summary>
        /// 
        /// </summary>
        public string Url { get; private set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> Headers { get; private set; } = new Dictionary<string, string>();

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> QueryParams { get; private set; } = new Dictionary<string, string>();

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> FormData { get; private set; } = new Dictionary<string, string>();

        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> FormFileData { get; private set; } = new Dictionary<string, string>();

        /// <summary>
        /// 
        /// </summary>
        public string Body { get; private set; } = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        public RequestOption Option { get; private set; } = new RequestOption();

        /// <summary>
        /// 
        /// </summary>
        public HttpRequestMessage Request { get; private set; } = new HttpRequestMessage();

        /// <summary>
        /// 
        /// </summary>
        public string ContentType { get; private set; } = $"application/json;utf-8";

        /// <summary>
        /// 
        /// </summary>
        public string MediaType { get; private set; } = "application/json";

        /// <summary>
        /// 
        /// </summary>
        public string CharSet { get; private set; } = "utf-8";

        /// <summary>
        /// 
        /// </summary>
        public Encoding RequestEncoding { get; private set; } = Encoding.UTF8;

        /// <summary>
        /// 
        /// </summary>
        public Encoding ResponseEncoding { get; private set; } = Encoding.UTF8;

        /// <summary>
        /// 请求拦截器
        /// </summary>
        public Action<HttpRequestMessage>? RequestInterceptor { get; set; }

        /// <summary>
        /// 响应拦截器
        /// </summary>
        public Action<HttpRequestMessage, HttpResponseMessage?>? ResponseInterceptor { get; set; }

        /// <summary>
        /// 请求失败判断过滤器
        /// </summary>
        public Func<HttpResponseMessage, bool> RequestFailedFilter { get; set; } = (resp) => resp.StatusCode != HttpStatusCode.OK;

        /// <summary>
        /// ctor
        /// </summary>
        public HttpUtils() => RequestReset();

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="Url"></param>
        public HttpUtils(string Url)
        {
            RequestReset();
            this.Url = Url;
        }

        /// <summary>
        /// 设置请求地址
        /// </summary>
        /// <param name="Url"></param>
        /// <returns></returns>
        public HttpUtils SetUrl(string Url)
        {
            this.Url = Url;
            return this;
        }

        /// <summary>
        /// 设置ContentType
        /// </summary>
        /// <param name="MediaType"></param>
        /// <param name="CharSet"></param>
        /// <returns></returns>
        public HttpUtils SetContentType(string MediaType = "application/json", string CharSet = "utf-8")
        {
            if (!MediaType.IsNullOrEmpty())
            {
                var array = ContentType.Split(';');
                this.MediaType = array[0];

            }

            if (!CharSet.IsNullOrEmpty())
            {
                this.CharSet = CharSet;
            }

            return this;
        }

        /// <summary>
        /// 设置请求超时时间(单位:秒)
        /// 默认：30秒
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public HttpUtils SetRequestTimeout(int timeout)
        {
            this.Option.Timeout = timeout;
            return this;
        }

        /// <summary>
        /// 添加请求头
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public HttpUtils AddRequestHeader(string key, string value)
        {
            if (this.ContentTypeKey.Contains(key.ToLower()))
                this.ContentType = value;

            this.Headers ??= new Dictionary<string, string>();
            this.Headers.Add(key, value);
            return this;
        }

        /// <summary>
        /// 添加Url请求键值对
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public HttpUtils AddQueryParams(string key, string value)
        {
            this.QueryParams ??= new Dictionary<string, string>();
            this.QueryParams.Add(key, value);
            return this;
        }

        /// <summary>
        /// 添加请求体
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public HttpUtils AddBody(string body)
        {
            this.Body = body;
            this.FormData = new Dictionary<string, string>();
            this.FormFileData = new Dictionary<string, string>();
            return this;
        }

        /// <summary>
        /// 添加请求体
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public HttpUtils AddJsonBody<T>(T body) where T : class
        {
            this.SetContentType();
            this.Body = body.ToJson();
            this.FormData = new Dictionary<string, string>();
            this.FormFileData = new Dictionary<string, string>();
            return this;
        }

        /// <summary>
        /// 添加表单
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public HttpUtils AddFormData(string key, string value)
        {
            this.FormData ??= new Dictionary<string, string>();
            this.FormData.Add(key, value);
            this.Body = string.Empty;
            return this;
        }

        /// <summary>
        /// 添加上传文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public HttpUtils AddFormFileData(string fileName, string filePath)
        {
            this.FormFileData ??= new Dictionary<string, string>();
            this.FormFileData.Add(fileName, filePath);
            this.Body = string.Empty;
            return this;
        }

        /// <summary>
        /// 添加HttpRequestMessage
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public HttpUtils AddHttpRequestMessage(Action<HttpRequestMessage> configure)
        {
            configure.Invoke(this.Request);
            return this;
        }

        /// <summary>
        /// 请求配置设置
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public HttpUtils RequestOptionBuilder(Action<RequestOption> configure)
        {
            configure.Invoke(this.Option);
            return this;
        }

        /// <summary>
        /// 设置请求体字符集编码
        /// </summary>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public HttpUtils SetRequestEncoding(string encoding)
        {
            this.RequestEncoding = Encoding.GetEncoding(encoding);
            return this;
        }

        /// <summary>
        /// 设置请求体字符集编码
        /// </summary>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public HttpUtils SetRequestEncoding(Encoding encoding)
        {
            this.RequestEncoding = encoding;
            return this;
        }

        /// <summary>
        /// 设置响应体字符集编码
        /// </summary>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public HttpUtils SetResponseEncoding(string encoding)
        {
            this.ResponseEncoding = Encoding.GetEncoding(encoding);
            return this;
        }

        /// <summary>
        /// 设置响应体字符集编码
        /// </summary>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public HttpUtils SetResponseEncoding(Encoding encoding)
        {
            this.ResponseEncoding = encoding;
            return this;
        }

        /// <summary>
        /// HttpGet请求
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResponse<string>> HttpGetAsync()
        {
            var result = new HttpResponse<string>();

            try
            {
                this.Request.Method = HttpMethod.Get;

                var res = await GetResponseAsync(Option, Request);

                if (res == null)
                {
                    result.Success = false;
                    return result;
                }

                result.HttpStatusCode = res.StatusCode;
                result.Success = (int)result.HttpStatusCode < 300;
                result.Content = await GetResponseBodyAsync(res.Content);
                return result;
            }
            catch (Exception ex)
            {
                result.Exception = ex;
                return result;
            }
        }

        /// <summary>
        /// HttpPost请求
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResponse<string>> HttpPostAsync()
        {
            var result = new HttpResponse<string>();

            try
            {
                this.Request.Method = HttpMethod.Post;

                var res = await GetResponseAsync(Option, Request);

                if (res == null)
                {
                    result.Success = false;
                    return result;
                }

                result.HttpStatusCode = res.StatusCode;
                result.Success = (int)result.HttpStatusCode < 300;
                result.Content = await GetResponseBodyAsync(res.Content);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Exception = ex;
            }

            return result;
        }

        /// <summary>
        /// HttpPut请求
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResponse<string>> HttpPutAsync()
        {
            var result = new HttpResponse<string>();

            try
            {
                this.Request.Method = HttpMethod.Put;

                var res = await GetResponseAsync(Option, Request);

                if (res == null)
                {
                    result.Success = false;
                    return result;
                }

                result.HttpStatusCode = res.StatusCode;
                result.Success = (int)result.HttpStatusCode < 300;
                result.Content = await GetResponseBodyAsync(res.Content);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Exception = ex;
            }

            return result;
        }

        /// <summary>
        /// HttpDelete请求
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResponse<string>> HttpDeleteAsync()
        {
            var result = new HttpResponse<string>();

            try
            {
                this.Request.Method = HttpMethod.Delete;

                var res = await GetResponseAsync(Option, Request);

                if (res == null)
                {
                    result.Success = false;
                    return result;
                }

                result.HttpStatusCode = res.StatusCode;
                result.Success = (int)result.HttpStatusCode < 300;
                result.Content = await GetResponseBodyAsync(res.Content);
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Exception = ex;
            }

            return result;
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <returns></returns>
        public Task<HttpResponse<byte[]>> DownloadAsync() => DownloadAsync(HttpMethod.Get);

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="httpMethod"></param>
        /// <returns></returns>
        public async Task<HttpResponse<byte[]>> DownloadAsync(HttpMethod httpMethod)
        {
            var result = new HttpResponse<byte[]>();

            try
            {
                this.Request.Method = httpMethod;

                var res = await GetResponseAsync(Option, Request);

                res.EnsureSuccessStatusCode();

                if (res == null)
                {
                    result.Success = false;
                    return result;
                }

                result.HttpStatusCode = res.StatusCode;
                result.Success = (int)result.HttpStatusCode < 300;
                result.Content = await res.Content.ReadAsByteArrayAsync();
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Exception = ex;
            }

            return result;
        }

        /// <summary>
        /// 统一请求封装
        /// </summary>
        /// <param name="options"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<HttpResponseMessage> GetResponseAsync(RequestOption options, HttpRequestMessage request)
        {
            HttpResponseMessage? result = null;
            var builder = new HttpUtilsBuilder();

            try
            {
                //
                MapHttpRequestMessage();

                //添加默认请求头
                AddDefaultHeader();

                var client = builder.Create(options);

                this.RequestInterceptor?.Invoke(request);

                result = await client.SendAsync(request);

                this.ResponseInterceptor?.Invoke(request, result);

                return result;
            }
            catch
            {
                throw;
            }
            finally
            {
                RequestReset();
            }
        }

        private void MapHttpRequestMessage()
        {
            if (this.QueryParams.Count > 0)
            {
                this.Url += "?";
                this.Url += this.QueryParams.ToAsciiSortParams();
            }

            this.Request.RequestUri = new Uri(this.Url);

            if (this.Body.IsNullOrEmpty() == false)
                this.Request.Content = new StringContent(this.Body, this.RequestEncoding);
            else if (FormData != null && this.FormData.Any() || this.FormFileData != null && this.FormFileData.Any())
            {
                var formContent = new MultipartFormDataContent();
                this.FormData!.ForEach(x => formContent.Add(new StringContent(x.Value, RequestEncoding), x.Key));
                this.FormFileData.ForEach(x => formContent.Add(new ByteArrayContent(File.ReadAllBytes(x.Value)), "file", x.Key));
                this.Request.Content = formContent;
            }
        }

        /// <summary>
        /// 添加默认请求头
        /// </summary>
        private void AddDefaultHeader()
        {
            // 主动添加的请求头
            if (this.Headers.HasValue())
                this.Headers.Where(x => !this.ContentTypeKey.Contains(x.Key.ToLower())).ForEach(x => this.Request.Headers.TryAddWithoutValidation(x.Key, x.Value));

            this.Request.Headers.TryAddWithoutValidation("Cache-Control", "no-cache");

            this.Request.Headers.TryAddWithoutValidation("Accept", "application/json");

            this.Request.Headers.TryAddWithoutValidation("User-Agent", _DefaultUserAgent);

            // ContentType
            if (this.Request.Content != null && this.Request.Method != HttpMethod.Get)
            {
                this.Request.Content.Headers.ContentType = new MediaTypeHeaderValue(this.MediaType.IsNullOrEmpty() ? "" : "application/json") { CharSet = CharSet };
            }

            var contentType = this.Headers.Where(x => this.ContentTypeKey.Contains(x.Key.ToLower())).SingleOrDefault();

            if (contentType.Value.IsNullOrEmpty())
            {
                if (this.Request.Method == HttpMethod.Post)
                    this.Request.Headers.TryAddWithoutValidation("ContentType", $"application/json; {CharSet}");
            }
            else
            {
                this.Request.Headers.TryAddWithoutValidation("ContentType", contentType.Value);
            }
        }

        /// <summary>
        /// 字符集编码
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private async Task<string?> GetResponseBodyAsync(HttpContent? content)
        {
            if (content == null)
                return null;

            string res;
            if (this.ResponseEncoding == null || this.ResponseEncoding == Encoding.UTF8)
                res = await content.ReadAsStringAsync();
            else
            {
                var bytes = await content.ReadAsByteArrayAsync();
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                res = HttpUtility.UrlDecode(bytes, this.ResponseEncoding);
            }

            try
            {
                //移除制表符
                return res.ToJsonString();
            }
            catch
            {
                return res;
            }
        }

        /// <summary>
        /// 重置请求
        /// </summary>
        private void RequestReset()
        {
            this.Url = string.Empty;
            this.Headers = new Dictionary<string, string>();
            this.QueryParams = new Dictionary<string, string>();
            this.Body = string.Empty;
            this.FormData = new Dictionary<string, string>();
            this.FormFileData = new Dictionary<string, string>();
            this.Request = new HttpRequestMessage();
            this.Option = new RequestOption();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class HttpResponse<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public HttpResponse()
        {
            Success = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Exception"></param>
        public HttpResponse(Exception Exception)
        {
            Success = false;
            this.Exception = Exception;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public T? Content { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public HttpStatusCode HttpStatusCode { get; set; } = HttpStatusCode.OK;

        /// <summary>
        /// 
        /// </summary>
        public Exception? Exception { get; set; }
    }
}
