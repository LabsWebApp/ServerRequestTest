using System;
using System.Net.Http;
using ServerRequestTest.SingleRequests.Exceptions;

namespace ServerRequestTest.SingleRequests
{
    public class SimpleHttp : Request
    {
        private const int MaxTextLength = 1000000;
        public SimpleHttp(string host, int count = 1)
        {
            Host = host;
            Count = count;
            Run = _ =>
            {
                try
                {
                    if (Cancel.IsCancellationRequested)
                        return;
                    HttpClient httpClient = new();
                    var result = httpClient.GetStringAsync(Host, Cancel).Result;
                    if (Cancel.IsCancellationRequested)
                        return;
                    if (result.Length > MaxTextLength)
                        throw new ContentException("Объём страницы превышает лимит.");
                }
                catch (Exception e)
                {
                    Error = e;
                    throw;
                }
            };
        }
    }
}
