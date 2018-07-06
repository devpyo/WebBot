using System;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium.Chrome;

namespace WebBot.Logic.PageDownload
{
    public class ChromeDriverDownloader : IHttpPageDownloader
    {
        ILogger<ChromeDriverDownloader> logger;

        public ChromeDriverDownloader(ILogger<ChromeDriverDownloader> logger)
        {
            this.logger = logger;
        }

        IObservable<string> IHttpPageDownloader.ReadPage(string url)
        {
            return Observable.Start(() =>
            {
                var options = new ChromeOptions();
                options.AddArguments("--headless");
                using (var driver = new ChromeDriver(@"./", options)) // TODO: 매번 만들면 안된다.
                {
                    driver.Url = url;
                    return driver.PageSource;
                }
            });
        }
    }
}
