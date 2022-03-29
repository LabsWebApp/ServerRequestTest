using static System.Console;
using System.Diagnostics;
using ServerRequestTestLibrary.SingleRequests;
using ServerRequestTestLibrary;
using ServerRequestTestLibrary.SingleRequests.Helper;

namespace ConsoleServerRequestTest;

/// <summary>
/// Провайдер управления теста
/// </summary>
internal class ConsoleProvider : Provider
{
    private ConsoleProvider(Request single) : base(single) { }

    #region Static Methods
    /// <summary>
    /// Статический запуск теста в консольном приложении
    /// </summary>
    /// <param name="request">конкретная оболочка единичных тестов</param>
    public static void RunInConsole(Request request)
    {
        using var provider = new ConsoleProvider(request);
        provider.RunInConsole();
    }

    /// <summary>
    /// Управление и отображение прогресса выполнения теста 
    /// </summary>
    /// <param name="test">задача тестирования</param>
    /// <param name="cancellation">сигнализация отмены тестирования</param>
    /// <param name="frequency">частота смены "кадров" в мс</param>
    private static void DisplayProgress(Task test, CancellationTokenSource cancellation, int frequency = 40)
    {
        void Escape(object? _, ConsoleCancelEventArgs consoleCancelEventArgs)
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
            ResetColor();
            CursorVisible = true;
        }
    }
    #endregion

    #region Method RunInConsole
    /// <summary>
    /// Запуск теста в консольном приложении
    /// </summary>
    private void RunInConsole()
    {
        WriteLine($"Стартуем тест (кол-во запросов: {Count}) ...");
        WriteLine("(Ctl+С - принудительно завершить тестирование)");
        Beep();
        var sw = Stopwatch.StartNew();
        var test = Run();
        ForegroundColor = ConsoleColor.DarkBlue;
        DisplayProgress(test, CancelSource);
        sw.Stop();
        Beep();
        var errorsOccurred = Exceptions?.Any() ?? false;
        if (test.IsFaulted || errorsOccurred)
        {
            ForegroundColor = ConsoleColor.Red;
            WriteLine($"[кол-во запросов: {Count}, из них успели начать: {Attempts}, успехов: {Successes}]");
            if (IsNotCancelled)
            {
                var exs = Exceptions?.Any() ?? false;

                if (test.Exception is Exception ex)
                {
                    WriteLine($"Ошибка в запросе: \n{GetErrorMessage.GetExceptionInfo(ex)}");
                    if (exs) WriteLine('\n');
                }
                if (exs) WriteLine(
                    $"ошибки в тестах:\n{GetErrorMessage.GetExceptionsListInfo(Exceptions)}");
            }
            else WriteLine("Тест принудительно прерван.");
        }
        else
        {
            ForegroundColor = ConsoleColor.Green;
            WriteLine($"[кол-во запросов: {Count}]\nТест прошёл успешно за {sw.Elapsed}.");
        }
        ResetColor();
        WriteLine();
    }
    #endregion
}