using System;

namespace ServerRequestTestLibrary.SingleRequests.Exceptions
{
    /// <summary>
    /// Представляет ошибку, к-ая возникает в случае, если не удалось подключится к SignalR - Hub
    /// </summary>
    public class HubConnectionException : Exception
    {
        /// <summary>
        /// Инициализация ошибки
        /// </summary>
        /// <param name="message">Сообщение о неудачи</param>
        public HubConnectionException(string message) : base(message) { }
    }
}
