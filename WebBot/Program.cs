using System;
using System.Net;
using System.Reactive.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using RestSharp;
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

        /// <summary>
        /// 
        /// </summary>

        static ServiceProvider BuildDi()
        {
            var services = new ServiceCollection();
            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            services.AddLogging((builder) => builder.SetMinimumLevel(LogLevel.Debug));

            services.AddTransient<ChromeWebDriver>();

            var serviceProvider = services.BuildServiceProvider();
            serviceProvider.GetRequiredService<ILoggerFactory>()
                .AddNLog(new NLogProviderOptions { CaptureMessageTemplates = true, CaptureMessageProperties = true });
            return serviceProvider;
        }

        static void RunTest_ChromeDriverWithDI()
        {
            using (var serviceProvider = BuildDi())
            {
                var logger = serviceProvider.GetService<ILogger<Program>>();
                logger.LogInformation("Main Run");

                var a = serviceProvider.GetService<ChromeWebDriver>();
                a.Do();
            }
        }

        /// <summary>
        /// 
        /// </summary>

        static IObservable<IRestResponse> RetryGetPageStream()
        {
            System.Console.WriteLine("stream started.");
            var restSharpClient = new RestClient("http://test.example.com/");
            var request = new RestRequest();
            return Observable.FromAsync(token => restSharpClient.ExecuteTaskAsync(request, token))
                .SelectMany(response =>
                {
                    System.Console.WriteLine($"response: {response.ErrorMessage}, {response.IsSuccessful}");

                    return response.IsSuccessful
                        ? Observable.Return(response)
                        : Observable.Throw<IRestResponse>(new Exception("Resource not ready.")).DelaySubscription(TimeSpan.FromSeconds(3));
                })
                .Retry(3);
                //.Catch<IRestResponse, Exception>(x => RetryGetPageStream());
        }

        static void RunTest_RestSharp()
        {
            try
            {
                RetryGetPageStream().Wait();
            }
            catch (Exception e)
            {
                System.Console.WriteLine(e);
            }
        }


        static void Main(string[] args)
        {
            //RunTest_GetPageWithCustomRequestAndSite();
            //RunTest_RestSharp();
            //RunTest_ChromeDriverWithDI();

            using (var startup = new Startup())
            {
                startup.Run(args, startUrl: "www.google.com");
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
