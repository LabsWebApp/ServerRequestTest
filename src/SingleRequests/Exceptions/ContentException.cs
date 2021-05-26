using System;

namespace ServerRequestTest.SingleRequests.Exceptions
{
    public class ContentException : Exception
    {
        public ContentException(string message) : base(message) { }
    }
}
