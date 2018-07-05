using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using WebBot.Logic.Infrastructure;

namespace WebBot.Logic.Http
{
    public class HttpClientDownloader
    {
        readonly CookieContainer CookieContainer = new CookieContainer();

        public HttpClientEntry CreateHttpClientEntry()
        {
            var AllowAutoRedirect = true;
            double _timeout = 8000;
            var httpClientEntry = new HttpClientEntry();
            httpClientEntry.Init(AllowAutoRedirect, () =>
            {
                if (!Equals(httpClientEntry.Client.Timeout.TotalSeconds, _timeout))
                {
                    httpClientEntry.Client.Timeout = new TimeSpan(0, 0, (int)_timeout / 1000);
                }
            }, CopyCookieContainer);
            return httpClientEntry;
        }

        public string ReadContent(Site site, HttpResponseMessage response)
        {
            byte[] contentBytes = response.Content.ReadAsByteArrayAsync().Result;
            contentBytes = PreventCutOff(contentBytes);
            if (string.IsNullOrWhiteSpace(site.EncodingName))
            {
                var charSet = response.Content.Headers.ContentType?.CharSet;
                Encoding htmlCharset = EncodingExtensions.GetEncoding(charSet, contentBytes);
                return htmlCharset.GetString(contentBytes, 0, contentBytes.Length);
            }
            else
            {
                return site.Encoding.GetString(contentBytes, 0, contentBytes.Length);
            }
        }

        byte[] PreventCutOff(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; i++)
            {
                if (bytes[i] == 0x00)
                {
                    bytes[i] = 32;
                }
            }
            return bytes;
        }

        CookieContainer CopyCookieContainer()
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, CookieContainer);
                stream.Seek(0, SeekOrigin.Begin);
                return (CookieContainer)formatter.Deserialize(stream);
            }
        }

        public HttpRequestMessage GenerateHttpRequestMessage(Request request, Site site)
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(request.Method ?? HttpMethod.Get, request.Url);

            var userAgentHeader = "User-Agent";
            httpRequestMessage.Headers.TryAddWithoutValidation(userAgentHeader, site.Headers.ContainsKey(userAgentHeader) ? site.Headers[userAgentHeader] : site.UserAgent);

            if (!string.IsNullOrWhiteSpace(request.Referer))
            {
                httpRequestMessage.Headers.TryAddWithoutValidation("Referer", request.Referer);
            }

            if (!string.IsNullOrWhiteSpace(request.Origin))
            {
                httpRequestMessage.Headers.TryAddWithoutValidation("Origin", request.Origin);
            }

            if (!string.IsNullOrWhiteSpace(site.Accept))
            {
                httpRequestMessage.Headers.TryAddWithoutValidation("Accept", site.Accept);
            }

            var contentTypeHeader = "Content-Type";

            foreach (var header in site.Headers)
            {
                if (header.Key.ToLower() == "cookie")
                {
                    continue;
                }
                if (!string.IsNullOrWhiteSpace(header.Key) && !string.IsNullOrWhiteSpace(header.Value) && header.Key != contentTypeHeader && header.Key != userAgentHeader)
                {
                    httpRequestMessage.Headers.TryAddWithoutValidation(header.Key, header.Value);
                }
            }

            if (httpRequestMessage.Method == HttpMethod.Post)
            {
                var data = string.IsNullOrWhiteSpace(site.EncodingName) ? Encoding.UTF8.GetBytes(request.PostBody) : site.Encoding.GetBytes(request.PostBody);
                httpRequestMessage.Content = new StreamContent(new MemoryStream(data));


                if (site.Headers.ContainsKey(contentTypeHeader))
                {
                    httpRequestMessage.Content.Headers.TryAddWithoutValidation(contentTypeHeader, site.Headers[contentTypeHeader]);
                }

                var xRequestedWithHeader = "X-Requested-With";
                if (site.Headers.ContainsKey(xRequestedWithHeader) && site.Headers[xRequestedWithHeader] == "NULL")
                {
                    httpRequestMessage.Content.Headers.Remove(xRequestedWithHeader);
                }
                else
                {
                    if (!httpRequestMessage.Content.Headers.Contains(xRequestedWithHeader) && !httpRequestMessage.Headers.Contains(xRequestedWithHeader))
                    {
                        httpRequestMessage.Content.Headers.TryAddWithoutValidation(xRequestedWithHeader, "XMLHttpRequest");
                    }
                }
            }
            return httpRequestMessage;
        }
    }
}
