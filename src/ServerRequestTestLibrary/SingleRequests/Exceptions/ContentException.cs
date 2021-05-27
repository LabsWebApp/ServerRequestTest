using System;

namespace ServerRequestTestLibrary.SingleRequests.Exceptions
{
    /// <summary>
    /// Представляет ошибку, к-ая возникает в случае, если размер контента единичного теста слишком велик
    /// </summary>
    public class ContentException : Exception
    {
        /// <summary>
        /// Инициализация ошибки
        /// </summary>
        /// <param name="message">Сообщение о превышении размера контента</param>
        public ContentException(string message) : base(message) { }
    }
}
