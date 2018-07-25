using System;

namespace WebBot.Logic.PageDownload
{
    public class PageDownloader
    {
        IHttpPageDownloader httpPageDownloader;

        public PageDownloader(IHttpPageDownloader httpPageDownloader)
        {
            this.httpPageDownloader = httpPageDownloader;
        }

        public IObservable<string> Download(string url)
        {
            return httpPageDownloader.ReadPage(url);
        }

        public string DownloadText(string url)
        {
            return httpPageDownloader.ReadText(url);
        }
    }
}
