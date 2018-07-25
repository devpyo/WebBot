using System;
using System.Reactive.Linq;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace WebBot.Logic.PageDownload
{
    public class RestSharpDownloader : IHttpPageDownloader
    {
        ILogger<RestSharpDownloader> logger;

        public RestSharpDownloader(ILogger<RestSharpDownloader> logger)
        {
            this.logger = logger;
        }

        IObservable<string> IHttpPageDownloader.ReadPage(string url)
        {
            var restSharpClient = new RestClient(url);
            var request = new RestRequest();
            return Observable.FromAsync(token => restSharpClient.ExecuteTaskAsync(request, token)).Select(x => x.Content);
        }

        string IHttpPageDownloader.ReadText(string url)
        {
            var restSharpClient = new RestClient(url);
            var request = new RestRequest();
            return restSharpClient.Execute(request).Content;
        }
    }
}
