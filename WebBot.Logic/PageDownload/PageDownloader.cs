namespace WebBot.Logic.PageDownload
{
    public class PageDownloader
    {
        IHttpPageDownloader httpPageDownloader;

        public PageDownloader(IHttpPageDownloader httpPageDownloader)
        {
            this.httpPageDownloader = httpPageDownloader;
        }

        public string Download(string url)
        {
            return httpPageDownloader.ReadPage(url);
        }
    }
}
