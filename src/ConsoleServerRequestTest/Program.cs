using ServerRequestTestLibrary;
using static System.Console;
using ServerRequestTestLibrary.SingleRequests;

namespace ConsoleServerRequestTest
{
    /// <summary>
    /// Точка входа
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
            int sw = ReadKey().KeyChar switch
            {
                '1' => 1,
                '2' => 2,
                _ => 0
            };
            Write("\b");
            
            WriteLine("Введите host:");
            var host = ReadLine();

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
            switch (ReadKey().KeyChar)
            {
                case 'R':
                case 'r':
                case 'К':
                case 'к':
                    Write("\b");
                    goto Repeat;
                case 'y':
                case 'Y':
                case 'н':
                case 'Н':
                    Write("\b");
                    goto Restart;
                case 'c':
                case 'C':
                case 'с':
                case 'С':
                    Write("\b");
                    goto Recount;
            }
        }
    }
}
