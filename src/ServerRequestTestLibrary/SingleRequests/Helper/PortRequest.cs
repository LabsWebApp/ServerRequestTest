using System;
using System.Linq;

namespace ServerRequestTestLibrary.SingleRequests.Helper
{
    public abstract class PortRequest : Request
    {
        private const int DefaultPort = 6666;
        protected const string DefaultHost = "127.0.0.1";
        
        protected int Port = DefaultPort;

        protected PortRequest(string host, int count)
        {
            Count = count;
            if (string.IsNullOrWhiteSpace(host))
            {
                Host = DefaultHost;
                return;
            }
            host = host.Trim();

            if (host.IndexOf(':') == 0)
            {
                Host = DefaultHost;
                host = host.Remove(0, 1);
                if (!int.TryParse(host, out Port))
                    Port = DefaultPort;
                return;
            }

            if (host.Count(c => c == '.') != 3) 
                throw new ArgumentException("Не верно указан host.");

            if (host.IndexOf(':') < 0)
            {
                Host = host;
                return;
            }

            var info = host.Split(":");
            if (info.Length != 2)
                throw new ArgumentException("Не верно указан host.");
            Host = info[0];
            if (!int.TryParse(info[1], out Port))
                throw new ArgumentException("Не верно указан port.");
        }
    }
}
