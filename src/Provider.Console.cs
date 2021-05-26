using System;
using static System.Console;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ServerRequestTest.SingleRequests;

namespace ServerRequestTest
{
    public partial class Provider
    {
        public static void RunInConsole(Request request)
        {
            using var provider = new Provider(request);
            provider.RunInConsole();
        }

        private partial void RunInConsole()
        {
            void Escape(object o, ConsoleCancelEventArgs consoleCancelEventArgs)
            {
                WriteLine("Принудительная остановка теста, подождите ...");
                _cancelSource.Cancel();
                consoleCancelEventArgs.Cancel = true;
            }

            WriteLine($"[кол-во запросов: {_single.Count}] Стартуем тест...");
            WriteLine("(Ctl+С-принудительно завершить тестирование)");
            Beep();
            ForegroundColor = ConsoleColor.DarkBlue;

            Stopwatch sw = Stopwatch.StartNew();
            var test = RunAsync();

            CancelKeyPress += Escape;
            CursorVisible = false;
            while (!test.IsCompleted)
            {
                Write("\\");
                Thread.Sleep(40);
                if (test.IsCompleted)
                {
                    Write("\b");
                    break;
                }
                Write("\b|");
                Thread.Sleep(40);
                if (test.IsCompleted)
                {
                    Write("\b");
                    break;
                }
                Write("\b/");
                Thread.Sleep(40);
                if (test.IsCompleted)
                {
                    Write("\b");
                    break;
                }
                Write("\b—");
                Thread.Sleep(40);
                Write("\b");
            }

            sw.Stop(); 
            CancelKeyPress -= Escape;
            CursorVisible = true;
            Beep();
            if (test.IsFaulted)
            {
                if (IsNotCancelled)
                {
                    Beep();
                    Beep();
                }
                ForegroundColor = ConsoleColor.Red;
                WriteLine($"[кол-во запросов: {_single.Count}, из них успехов: {Successes}]");
                WriteLine(IsNotCancelled ?
                    $"Ошибка в запросе: {test.Exception?.Message ?? "\b\b/"}.":
                    "Тест принудительно прерван.");
            }
            else
            {
                ForegroundColor = ConsoleColor.Green;
                WriteLine($"[кол-во запросов: {_single.Count}]\nТест прошёл успешно за {sw.Elapsed}.");
            }
            ResetColor();
        }
    }
}
