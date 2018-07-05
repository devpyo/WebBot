using Microsoft.Extensions.Logging;

namespace WebBot.Logic
{
    public class WebBot
    {
        ILogger<WebBot> logger;

        public WebBot(ILogger<WebBot> logger)
        {
            this.logger = logger;
        }

        public void Run(string startUrl)
        {
            logger.LogInformation($"WebBot Run : {startUrl}");
        }
    }
}
