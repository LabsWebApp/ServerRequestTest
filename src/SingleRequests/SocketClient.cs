using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using ServerRequestTest.SingleRequests.Exceptions;

namespace ServerRequestTest.SingleRequests
{
    public enum SocketDataMode
    {
        Guid,
        Int,
        GuidAndInt
    }
    public class SocketClient : Request
    {
        private const int DefaultPort = 6666;
        private readonly int _port = DefaultPort;//10.4.80.2
        public SocketClient(
            string host,
            int count = 1,
            SocketDataMode mode = SocketDataMode.Int)
        {
            var colon = host.IndexOf(':');
            if (string.IsNullOrWhiteSpace(host))
            {
                Host = "127.0.0.1";
            }
            else if (colon == 0)
            {
                Host = "127.0.0.1";
                if (!int.TryParse(host.Remove(0, 1), out _port))
                    _port = DefaultPort;
            }
            else if (colon < 0)
            {
                Host = host.Trim();
                _port = DefaultPort;
            }

            if (!string.IsNullOrWhiteSpace(host) && colon > 0) 
            {
                var hostPort = host.Split(':');
                if (hostPort.Length == 2)
                {
                    Host = hostPort[0].Trim();
                    if (!int.TryParse(hostPort[1].Trim(), out _port))
                        _port = DefaultPort;
                }
            }

            Count = count;
            Run = i =>
            {
                try
                {
                    if (Cancel.IsCancellationRequested)
                        return;
                    if (!Host.Contains('.'))
                        throw new ArgumentException("Не верно указан host.");
                    using TcpClient client = new(Host, _port);
                    IPGlobalProperties ipProperties =
                        IPGlobalProperties.GetIPGlobalProperties();
                    var tcpConnections = ipProperties
                        .GetActiveTcpConnections()
                        .Where(x =>
                            x.LocalEndPoint.Equals(client.Client.LocalEndPoint) &&
                            x.RemoteEndPoint.Equals(client.Client.RemoteEndPoint))
                        .ToArray();
                    if (tcpConnections.Any())
                    {
                        TcpState stateOfConnection = tcpConnections.First().State;
                        if (stateOfConnection != TcpState.Established)
                        {
                            Error = new ArgumentException($"Нет соединения с {Host}:{_port}.");
                            throw Error;
                        }
                    }
                    else
                    {
                        Error = new ArgumentException($"Не верно указаны - {Host}:{_port}.");
                        throw Error;
                    }

                    string data = mode switch
                    {
                        SocketDataMode.Int => i.ToString(),
                        _ => string.Empty
                    };
                    var stream = client.GetStream();
                    byte[] buffer = Encoding.UTF8.GetBytes(data);
                    stream.WriteAsync(buffer, 0, buffer.Length, Cancel);
                    stream.FlushAsync(Cancel);
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
