using Microsoft.Extensions.Logging;
using WebBot.Logic.PageDownload;

namespace WebBot.Logic
{
    public class WebBot
    {
        ILogger<WebBot> logger;
        PageDownloader pageDownloader;

        public WebBot(ILogger<WebBot> logger, PageDownloader pageDownloader)
        {
            this.logger = logger;
            this.pageDownloader = pageDownloader;
        }

        public void Run(string startUrl)
        {
            logger.LogInformation($"WebBot Run : {startUrl}");

            logger.LogDebug($"page = {pageDownloader.Download(startUrl).Substring(0, 100)}");
        }
    }
}
