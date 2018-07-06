using System;

namespace WebBot.Logic.PageDownload
{
    public interface IHttpPageDownloader
    {
        IObservable<string> ReadPage(string url);
    }
}
