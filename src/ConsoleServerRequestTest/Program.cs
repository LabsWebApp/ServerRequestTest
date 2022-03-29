using static System.Console;
using ServerRequestTestLibrary.SingleRequests;
using ServerRequestTestLibrary.SingleRequests.Helper;

namespace ConsoleServerRequestTest;

/// <summary>
/// Класс с точкой входа
/// </summary>
internal class Program
{
    /// <summary>
    /// Управление запуском тестирования
    /// </summary>
    static void Main()
    {
        Restart:
        WriteLine("Выберите\n'1' - SocketClient\n'2' - SignalRClient\nлюбая клавиша - SimpleHttp):");
        var sw = ReadKey().KeyChar switch
        {
            '1' => 1,
            '2' => 2,
            _ => 0
        };
        Write("\b");

        WriteLine("Введите host:");
        var host = ReadLine();
        if (string.IsNullOrWhiteSpace(host))
        {
            WriteLine("Невозможно прочесть host, попытайтесь ещё раз!");
            goto Restart;
        }

        Recount:
        WriteLine("Введите количество тестов:");
        if (!int.TryParse(ReadLine() ?? "", out var count))
            count = 1;

        Repeat:
        Request source = sw switch
        {
            1 => new SocketClient(host, count),
            2 => new SignalRClient(host, count),
            _ => new SimpleHttp(host, count)
        };

        ConsoleProvider.RunInConsole(source);

        WriteLine("Провести новый тест?\n(r - повторить предыдущий; c - изменить кол-во тестов; y - новый, выход - любая клавиша)");
        switch (ReadKey())
        {
            case { Key: ConsoleKey.R }:
                Write("\b");
                goto Repeat;
            case { Key: ConsoleKey.Y }:
                Write("\b");
                goto Restart;
            case { Key: ConsoleKey.C }:
                Write("\b");
                goto Recount;
            default:
                return;
        }
    }
}