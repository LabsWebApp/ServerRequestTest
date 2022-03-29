using Microsoft.AspNetCore.SignalR.Client;
using ServerRequestTestLibrary.SingleRequests.Exceptions;
using ServerRequestTestLibrary.SingleRequests.Helper;

namespace ServerRequestTestLibrary.SingleRequests;

public class SignalRClient : PortRequest
{
    private string HubClassName { get; init; }
    private string HubMethodName { get; init; }

    public SignalRClient(
        string host,
        int count = 1,
        DataMode mode = DataMode.Int) : base(host, count)
    {
        HubClassName = "ChatHub";
        HubMethodName = "SendMessage";
        if (Host.StartsWith("127."))
            Host = "localhost";
        Host = $"http://{Host}:{Port}";
        var connection = new HubConnectionBuilder()
            .WithUrl($"{Host}/{HubClassName}")
            .Build();
            
        connection.Closed += async _ =>
        {
            await Task.Delay(new Random().Next(1, 5) * 100);
            await connection.StartAsync();
        };

        Run = i =>
        {
            try
            {
                while (connection.State != HubConnectionState.Connected)
                {
                    if (Cancel.IsCancellationRequested)
                        return;
                    if (connection.State == HubConnectionState.Connecting)
                    {
                        Thread.Sleep(50);
                        break;
                    }
                    throw new HubConnectionException($"Невозможно подключиться к {Host}/{HubClassName}");
                }

                if (mode != DataMode.Int) throw new ResponseException();
                connection.InvokeAsync(HubMethodName,
                    i,null, Cancel);
            }
            catch (Exception e)
            {
                Error = e;
                throw;
            }
        };
    }
}