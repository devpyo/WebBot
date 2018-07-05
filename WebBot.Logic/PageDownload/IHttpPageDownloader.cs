namespace WebBot.Logic.PageDownload
{
    public interface IHttpPageDownloader
    {
        string ReadPage(string url);
    }
}
