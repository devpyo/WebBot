﻿using System;
using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using WebBot.Logic;
using WebBot.Logic.Http;
using WebBot.Logic.PageDownload;

namespace WebBot.Console
{
    class Program
    {
        static readonly CookieContainer CookieContainer = new CookieContainer();

        static void RunTest_GetPageWithCustomRequestAndSite()
        {
            var downloader = new HttpClientDownloader();
            var httpClientEntry = downloader.CreateHttpClientEntry();

            var request = new Request("http://google.co.kr");
            var site = new Site();
            var httpMessage = downloader.GenerateHttpRequestMessage(request, site);
            var result = httpClientEntry.Client.SendAsync(httpMessage).Result;

            System.Console.WriteLine($"[{result.StatusCode}] {downloader.ReadContent(site, result).Substring(0, 500)}");
        }

        static void Main(string[] args)
        {
            //RunTest_GetPageWithCustomRequestAndSite();

            using (var startup = new Startup())
            {
                var url = "http://bbs.ruliweb.com/hobby/board/300018/";
                startup.Run(args, startUrl: url);
            }
        }
    }

    public class Startup : IDisposable
    {
        readonly ServiceProvider serviceProvider;

        public Startup()
        {
            serviceProvider = BuildDi();
        }

        ServiceProvider BuildDi()
        {
            var services = new ServiceCollection();
            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddLogging((builder) => builder.SetMinimumLevel(LogLevel.Debug));

            services.AddTransient<Logic.WebBot>();
            services.AddTransient<PageDownloader>();
            //services.AddTransient<IHttpPageDownloader, ChromeDriverDownloader>();
            services.AddTransient<IHttpPageDownloader, RestSharpDownloader>();

            var serviceProvider = services.BuildServiceProvider();
            serviceProvider.GetRequiredService<ILoggerFactory>()
                .AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true });
            return serviceProvider;
        }

        public void Run(string[] args, string startUrl) // TODO: configuration으로 받아온다.
        {
            var logger = serviceProvider.GetService<ILogger<Program>>();
            logger.LogInformation("Startup Run");

            var bot = serviceProvider.GetService<Logic.WebBot>();
            bot.Run(startUrl);
        }

        void IDisposable.Dispose()
        {
            serviceProvider.Dispose();
        }
    }
}
