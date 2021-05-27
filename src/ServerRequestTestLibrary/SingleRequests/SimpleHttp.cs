using System;
using System.Net.Http;
using ServerRequestTestLibrary.SingleRequests.Exceptions;

namespace ServerRequestTestLibrary.SingleRequests
{
    /// <summary>
    /// Оболочка http(s) запроса 
    /// </summary>
    public class SimpleHttp : Request
    {
        /// <summary>
        /// Ограничение на размер текста контента
        /// </summary>
        private const int MaxTextLength = 1000000;

        /// <summary>
        /// Инициализация оболочки http(s) запроса
        /// </summary>
        /// <param name="host">URL или IP запроса</param>
        /// <param name="count">кол-во тестов</param>
        public SimpleHttp(string host, int count = 1)
        {
            Host = host;
            Count = count;
            Run = _ =>
            {
                if (Cancel.IsCancellationRequested)
                    return;
                try
                {
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
