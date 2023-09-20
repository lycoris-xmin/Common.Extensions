using Lycoris.Base.Extensions;
using Lycoris.Base.Http.Options;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace Lycoris.Base.Http
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
        public string JsonBody { get; private set; } = string.Empty;

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
        /// 
        /// </summary>
        public string UserAgent { get; private set; } = string.Empty;

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
            Option.Timeout = timeout;
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
            if (ContentTypeKey.Contains(key.ToLower()))
                this.ContentType = value;

            Headers ??= new Dictionary<string, string>();
            Headers.Add(key, value);
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
            QueryParams ??= new Dictionary<string, string>();
            QueryParams.Add(key, value);
            return this;
        }

        /// <summary>
        /// 添加请求体
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public HttpUtils AddJsonBody(string body)
        {
            JsonBody = body;
            FormData = new Dictionary<string, string>();
            FormFileData = new Dictionary<string, string>();
            return this;
        }

        /// <summary>
        /// 添加请求体
        /// </summary>
        /// <param name="body"></param>
        /// <returns></returns>
        public HttpUtils AddJsonBody<T>(T body) where T : class
        {
            JsonBody = body.ToJson();
            FormData = new Dictionary<string, string>();
            FormFileData = new Dictionary<string, string>();
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
            FormData ??= new Dictionary<string, string>();
            FormData.Add(key, value);
            JsonBody = string.Empty;
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
            FormFileData ??= new Dictionary<string, string>();
            FormFileData.Add(fileName, filePath);
            JsonBody = string.Empty;
            return this;
        }

        /// <summary>
        /// 添加HttpRequestMessage
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public HttpUtils AddHttpRequestMessage(Action<HttpRequestMessage> configure)
        {
            configure.Invoke(Request);
            return this;
        }

        /// <summary>
        /// 请求配置设置
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        public HttpUtils RequestOptionBuilder(Action<RequestOption> configure)
        {
            configure.Invoke(Option);
            return this;
        }

        /// <summary>
        /// 设置请求体字符集编码
        /// </summary>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public HttpUtils SetRequestEncoding(string encoding)
        {
            RequestEncoding = Encoding.GetEncoding(encoding);
            return this;
        }

        /// <summary>
        /// 设置请求体字符集编码
        /// </summary>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public HttpUtils SetRequestEncoding(Encoding encoding)
        {
            RequestEncoding = encoding;
            return this;
        }

        /// <summary>
        /// 设置响应体字符集编码
        /// </summary>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public HttpUtils SetResponseEncoding(string encoding)
        {
            ResponseEncoding = Encoding.GetEncoding(encoding);
            return this;
        }

        /// <summary>
        /// 设置响应体字符集编码
        /// </summary>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public HttpUtils SetResponseEncoding(Encoding encoding)
        {
            ResponseEncoding = encoding;
            return this;
        }

        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="UserAgent"></param>
        /// <returns></returns>
        public HttpUtils SetUserAgent(string UserAgent)
        {
            this.UserAgent = UserAgent;
            return this;
        }

        /// <summary>
        /// HttpGet请求
        /// </summary>
        /// <returns></returns>
        public async Task<HttpResponse> HttpGetAsync()
        {
            var result = new HttpResponse();

            try
            {
                Request.Method = HttpMethod.Get;

                var res = await GetResponseAsync(Option, Request);

                if (res == null)
                {
                    result.Success = false;
                    return result;
                }

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
        public async Task<HttpResponse> HttpPostAsync()
        {
            var result = new HttpResponse();

            try
            {
                Request.Method = HttpMethod.Post;

                var res = await GetResponseAsync(Option, Request);

                if (res == null)
                {
                    result.Success = false;
                    return result;
                }

                result.HttpStatusCode = res.StatusCode;

                if (result.HttpStatusCode != HttpStatusCode.OK)
                    result.Success = false;
                else
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
        public async Task<HttpResponse> HttpPutAsync()
        {
            var result = new HttpResponse();

            try
            {
                Request.Method = HttpMethod.Put;

                var res = await GetResponseAsync(Option, Request);

                if (res == null)
                {
                    result.Success = false;
                    return result;
                }

                result.HttpStatusCode = res.StatusCode;

                if (result.HttpStatusCode != HttpStatusCode.OK)
                    result.Success = false;
                else
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
        public async Task<HttpResponse> HttpDeleteAsync()
        {
            var result = new HttpResponse();

            try
            {
                Request.Method = HttpMethod.Delete;

                var res = await GetResponseAsync(Option, Request);

                if (res == null)
                {
                    result.Success = false;
                    return result;
                }

                result.HttpStatusCode = res.StatusCode;

                if (result.HttpStatusCode != HttpStatusCode.OK)
                    result.Success = false;
                else
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
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        private async Task<HttpResponseMessage> GetResponseAsync(RequestOption options, HttpRequestMessage request)
        {
            var builder = new HttpUtilsBuilder();

            try
            {
                //
                MapHttpRequestMessage();

                //添加默认请求头
                AddDefaultHeader();

                return await builder.Create(options).SendAsync(request);
            }
            catch (Exception)
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
            if (QueryParams.Count > 0)
            {
                Url += "?";
                Url += QueryParams.ToAsciiSortParams();
            }

            Request.RequestUri = new Uri(Url);

            if (JsonBody.IsNullOrEmpty() == false)
                Request.Content = new StringContent(JsonBody, RequestEncoding);
            else if (FormData != null && FormData.Any() || FormFileData != null && FormFileData.Any())
            {
                var formContent = new MultipartFormDataContent();
                FormData!.ForEach(x => formContent.Add(new StringContent(x.Value, RequestEncoding), x.Key));
                FormFileData.ForEach(x => formContent.Add(new ByteArrayContent(File.ReadAllBytes(x.Value)), "file", x.Key));
                Request.Content = formContent;
            }
        }

        /// <summary>
        /// 添加默认请求头
        /// </summary>
        private void AddDefaultHeader()
        {
            if (Request.Content == null)
                Request.Headers.TryAddWithoutValidation("Cache-Control", "no-cache");
            else if (Request.Content.Headers.Any(x => x.Key == "Cache-Control" && x.Value != null) == false)
                Request.Content.Headers.TryAddWithoutValidation("Cache-Control", "no-cache");

            if (Request.Content == null)
                Request.Headers.TryAddWithoutValidation("Accept", "application/json");
            else if (Request.Content.Headers.Any(x => x.Key == "Accept" && x.Value != null) == false)
                Request.Content.Headers.TryAddWithoutValidation("Accept", "application/json");

            if (Request.Content == null)
            {
                if (UserAgent.IsNullOrEmpty() == false)
                    Request.Headers.TryAddWithoutValidation("User-Agent", UserAgent);
                else if (Request.Headers.Any(x => x.Key == "User-Agent" && x.Value != null) == false)
                    Request.Headers.TryAddWithoutValidation("User-Agent", _DefaultUserAgent);
            }
            else
            {
                if (UserAgent.IsNullOrEmpty() == false)
                    Request.Content.Headers.TryAddWithoutValidation("User-Agent", UserAgent);
                else if (Request.Content.Headers.Any(x => x.Key == "User-Agent" && x.Value != null) == false)
                    Request.Content.Headers.TryAddWithoutValidation("User-Agent", _DefaultUserAgent);
            }

            // 主动添加的请求头
            if (Headers.HasValue())
            {
                Headers.Where(x => ContentTypeKey.Contains(x.Key.ToLower())).ForEach(x =>
                {
                    if (Request.Content == null)
                        Request.Headers.TryAddWithoutValidation(x.Key, x.Value);
                    else
                        Request.Content.Headers.TryAddWithoutValidation(x.Key, x.Value);
                });
            }

            // ContentType
            if (Request.Content != null && Request.Method != HttpMethod.Get)
                Request.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaType.IsNullOrEmpty() ? "" : "application/json") { CharSet = CharSet };
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
            if (ResponseEncoding == null || ResponseEncoding == Encoding.UTF8)
                res = await content.ReadAsStringAsync();
            else
            {
                var bytes = await content.ReadAsByteArrayAsync();
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                res = HttpUtility.UrlDecode(bytes, ResponseEncoding);
            }
            //移除制表符
            return res.ToJsonString();
        }

        /// <summary>
        /// 重置请求
        /// </summary>
        private void RequestReset()
        {
            Url = string.Empty;
            Headers = new Dictionary<string, string>();
            QueryParams = new Dictionary<string, string>();
            JsonBody = string.Empty;
            FormData = new Dictionary<string, string>();
            FormFileData = new Dictionary<string, string>();
            Request = new HttpRequestMessage();
            Option = new RequestOption();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class HttpResponse
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
        public string? Content { get; set; }

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
