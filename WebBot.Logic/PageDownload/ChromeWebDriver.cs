using Microsoft.Extensions.Logging;
using OpenQA.Selenium.Chrome;

namespace WebBot.Logic.PageDownload
{
    public class ChromeWebDriver : IHttpPageDownloader
    {
        ILogger<ChromeWebDriver> logger;

        public ChromeWebDriver(ILogger<ChromeWebDriver> logger)
        {
            this.logger = logger;
        }

        public void Do()
        {

        }


        public string ReadPage(string url)
        {
            var options = new ChromeOptions();
            options.AddArguments("--headless");
            using (var driver = new ChromeDriver(@"./", options))
            {
                driver.Url = url;
                return driver.PageSource;

                // 웹 드라이버
                //driver.Url = "http://bbs.ruliweb.com/hobby/board/300018/";
                //logger.LogInformation("Title = {0}", driver.Title);

                //// 어질리티 팩

                //var doc = new HtmlDocument();
                //doc.LoadHtml(driver.PageSource);
                //var nodes = doc
                //    .DocumentNode
                //    .SelectNodes(@"//*[@id=""board_list""]/div/div[2]/table/tbody/tr[*]");

                //File.WriteAllText(@"d:/test.txt", driver.PageSource);

                //var items = new List<string>();
                //foreach (var node in nodes)
                //{
                //    var s = node.SelectSingleNode(@".//td[@class=""subject""]/div/a");
                //    if (s == null)
                //        continue;

                //    items.Add(s.InnerText);
                //}

                //items.ForEach(x => logger.LogInformation(x));
            }
        }
    }
}
