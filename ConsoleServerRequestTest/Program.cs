using ServerRequestTest;
using static System.Console;
using ServerRequestTest.SingleRequests;

namespace ConsoleServerRequestTest
{
    class Program
    {
        //http://microsoft.com/
        static void Main()
        { 
            Restart:
            WriteLine("Выберите ('1' - SocketClient, любая клавиша - SimpleHttp):");
            int sw = ReadKey().KeyChar switch
            {
                '1' => 1,
                _ => 0
            };
            Write("\b");
            
            WriteLine("Введите host:");
            var host = ReadLine();

            Recount:
            WriteLine("Введите количество тестов:");
            if (!int.TryParse(ReadLine() ?? "", out int count))
                count = 1;

            Repeat:
            Request source = sw switch
            {
                1 => new SocketClient(host, count),
                _ => new SimpleHttp(host, count)
            };
            Provider.RunInConsole(source);

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
