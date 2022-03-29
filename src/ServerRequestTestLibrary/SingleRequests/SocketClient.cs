using ServerRequestTestLibrary.SingleRequests.Helper;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace ServerRequestTestLibrary.SingleRequests;

/// <summary>
/// Оболочка Socket-запроса со стороны клиента
/// </summary>
public class SocketClient : PortRequest
{
    public SocketClient(
        string host,
        int count = 1,
        DataMode mode = DataMode.Int) : base(host, count)
    {
        Run = i =>
        {
            try
            {
                if (Cancel.IsCancellationRequested)
                    return;
                using TcpClient client = new(Host, Port);
                var ipProperties =
                    IPGlobalProperties.GetIPGlobalProperties();
                var tcpConnections = ipProperties
                    .GetActiveTcpConnections()
                    .Where(x =>
                        x.LocalEndPoint.Equals(client.Client.LocalEndPoint) &&
                        x.RemoteEndPoint.Equals(client.Client.RemoteEndPoint))
                    .ToArray();
                if (tcpConnections.Any())
                {
                    var stateOfConnection = tcpConnections.First().State;
                    if (stateOfConnection != TcpState.Established)
                    {
                        Error = new ArgumentException($"Нет соединения с {Host}:{Port}.");
                        throw Error;
                    }
                }
                else
                {
                    Error = new ArgumentException($"Не верно указаны - {Host}:{Port}.");
                    throw Error;
                }

                var stream = client.GetStream();
                var buffer = Encoding.UTF8.GetBytes(SetData(i, mode));
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