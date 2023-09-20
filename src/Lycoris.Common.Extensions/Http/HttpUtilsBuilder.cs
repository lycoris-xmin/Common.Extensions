using Lycoris.Base.Http.Options;
using System.Net;

namespace Lycoris.Base.Http
{
    /// <summary>
    /// 
    /// </summary>
    internal class HttpUtilsBuilder
    {
        /// <summary>
        /// 超时时间
        /// </summary>
        private int DefaultTimeout;

        /// <summary>
        /// 
        /// </summary>
        public HttpUtilsBuilder()
        {
            DefaultTimeout = 30;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="option"></param>
        /// <returns></returns>
        public HttpClient Create(RequestOption option)
        {
            try
            {
                var handler = CreateHttpClientHandler(option);

                if (option.Timeout.HasValue && option.Timeout.Value >= 1)
                    DefaultTimeout = option.Timeout.Value;

                return new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(DefaultTimeout) };
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        private HttpClientHandler CreateHttpClientHandler(RequestOption options)
        {
            var handler = options.HttpHandlerOption.UseCookieContainer ? UseCookiesHandler(options, options.HttpMessageHandler) : UseNonCookiesHandler(options, options.HttpMessageHandler);

            if (options.DangerousAcceptAnyServerCertificateValidator)
                handler.ServerCertificateCustomValidationCallback = (request, certificate, chain, errors) => true;

            //响应压缩处理
            handler.AutomaticDecompression = options.AutomaticDecompression;

            return handler;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="route"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        private HttpClientHandler UseNonCookiesHandler(RequestOption route, HttpClientHandler? handler)
        {
            handler ??= new HttpClientHandler();

            handler.AllowAutoRedirect = route.HttpHandlerOption.AllowAutoRedirect;
            handler.UseCookies = route.HttpHandlerOption.UseCookieContainer;
            handler.UseProxy = route.HttpHandlerOption.UseProxy;
            handler.MaxConnectionsPerServer = route.HttpHandlerOption.MaxConnectionsPerServer;

            return handler;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="route"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        private HttpClientHandler UseCookiesHandler(RequestOption route, HttpClientHandler? handler)
        {
            handler ??= new HttpClientHandler();

            handler.AllowAutoRedirect = route.HttpHandlerOption.AllowAutoRedirect;
            handler.UseCookies = route.HttpHandlerOption.UseCookieContainer;
            handler.UseProxy = route.HttpHandlerOption.UseProxy;
            handler.MaxConnectionsPerServer = route.HttpHandlerOption.MaxConnectionsPerServer;
            handler.CookieContainer = route.HttpHandlerOption.Cookies ?? new CookieContainer();

            return handler;
        }
    }
}
