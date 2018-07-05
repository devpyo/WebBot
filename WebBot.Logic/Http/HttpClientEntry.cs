using System;
using System.Net;
using System.Net.Http;

namespace WebBot.Logic.Http
{
    public class HttpClientEntry
    {
        public HttpClient Client { get; private set; }
        public HttpClientHandler Handler { get; private set; }
        public DateTime ActiveTime { get; set; }
        bool initialized;

        public void Init(bool allowAutoRedirect, Action configAction, Func<CookieContainer> cookieContainerFactory)
        {
            if (initialized)
            {
                return;
            }

            Handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
                UseProxy = true,
                UseCookies = true,
                AllowAutoRedirect = true,
                MaxAutomaticRedirections = 10
            };
            Client = allowAutoRedirect ? new HttpClient(new GlobalRedirectHandler(Handler)) : new HttpClient(Handler);
            ActiveTime = DateTime.Now;

            configAction();

            Handler.CookieContainer = cookieContainerFactory();

            initialized = true;
        }
    }
}
