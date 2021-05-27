using System;
using static System.Console;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using ServerRequestTest.SingleRequests;

namespace ServerRequestTest
{
    public partial class Provider
    {
        #region Static Methods
        public static void RunInConsole(Request request)
        {
            using var provider = new Provider(request);
            provider.RunInConsole();
        }
        private static void DisplayProgress(Task test, CancellationTokenSource cancellation, int frequency = 40)
        {
            void Escape(object o, ConsoleCancelEventArgs consoleCancelEventArgs)
            {
                Write("\b");
                WriteLine("Принудительная остановка теста, подождите ...");
                cancellation.Cancel();
                consoleCancelEventArgs.Cancel = true;
            }

            CancelKeyPress += Escape;
            CursorVisible = false;

            try
            {
                while (!test.IsCompleted)
                {
                    Write("\\");
                    Thread.Sleep(frequency);
                    if (test.IsCompleted)
                    {
                        Write("\b");
                        break;
                    }

                    Write("\b|");
                    Thread.Sleep(frequency);
                    if (test.IsCompleted)
                    {
                        Write("\b");
                        break;
                    }

                    Write("\b/");
                    Thread.Sleep(frequency);
                    if (test.IsCompleted)
                    {
                        Write("\b");
                        break;
                    }

                    Write("\b—");
                    Thread.Sleep(frequency);
                    Write("\b");
                }
            }
            finally
            {
                CancelKeyPress -= Escape;
                CursorVisible = true;
            }
        }
        #endregion

        #region Method RunInConsole
        private partial void RunInConsole()
        {
            WriteLine($"[кол-во запросов: {_single.Count}] Стартуем тест...");
            WriteLine("(Ctl+С-принудительно завершить тестирование)");
            Beep();
            Stopwatch sw = Stopwatch.StartNew();
            var test = Run();
            ForegroundColor = ConsoleColor.DarkBlue;
            DisplayProgress(test, _cancelSource);
            sw.Stop();

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
                    $"Ошибка в запросе: {test.Exception?.Message ?? "\b\b/"}." :
                    "Тест принудительно прерван.");
            }
            else
            {
                ForegroundColor = ConsoleColor.Green;
                WriteLine($"[кол-во запросов: {_single.Count}]\nТест прошёл успешно за {sw.Elapsed}.");
            }
            ResetColor();
            WriteLine();
        }
        #endregion
    }
}
