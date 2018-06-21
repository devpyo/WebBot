using System;
using System.Net;
using WebBot.Http;

namespace WebBot
{
    class Program
    {
        static readonly CookieContainer CookieContainer = new CookieContainer();

        static void Main(string[] args)
        {
            var downloader = new HttpClientDownloader();
            var httpClientEntry = downloader.CreateHttpClientEntry();

            var request = new Request("http://google.co.kr");
            var site = new Site();
            var httpMessage = downloader.GenerateHttpRequestMessage(request, site);
            var result = httpClientEntry.Client.SendAsync(httpMessage).Result;

            Console.WriteLine($"[{result.StatusCode}] {downloader.ReadContent(site, result).Substring(0, 500)}");
        }
    }
}
