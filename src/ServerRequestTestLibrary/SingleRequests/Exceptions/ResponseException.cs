using System.Net;

namespace ServerRequestTestLibrary.SingleRequests.Exceptions;

/// <summary>
/// Представляет ошибку, к-ая возникает в случае получения не положительного ответа на запрос
/// </summary>
public class ResponseException : Exception
{
    /// <summary>
    /// Инициализация ошибки
    /// </summary>
    /// <param name="code">код ответа сервера</param>
    public ResponseException(HttpStatusCode? code = null)
        : base(code is null or HttpStatusCode.OK ? "Unknown error":
            $"Error occurred, the status code is: {code}") { }
}