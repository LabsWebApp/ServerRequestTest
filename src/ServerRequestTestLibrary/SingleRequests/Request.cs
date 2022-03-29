using ServerRequestTestLibrary.SingleRequests.Helper;

namespace ServerRequestTestLibrary.SingleRequests;

/// <summary>
/// Абстрактная оболочка для единичных запросов, проверяемого соединения
/// </summary>
public abstract class Request
{
    /// <summary>
    /// Резервация свободных потоков в ThreadPool
    /// </summary>
    public const int Reserve = 10;

    private ushort _count;
        
    /// <summary>
    /// Кол-во параллельных тестов
    /// </summary>
    public int Count
    {
        get => _count;
        set
        {
            if (value <= 0)
            {
                _count = 1;
                return;
            }
            ThreadPool.GetAvailableThreads(out var workerThreads, out var _);
            var setter = Math.Min(workerThreads, ushort.MaxValue);
            _count = value > setter ? (ushort)(setter - Reserve) : (ushort)value;
        }
    }

    /// <summary>
    /// Инкапсуляция запуска единичного теста
    /// </summary>
    public Action<int> Run { get; init; } = null!;

    /// <summary>
    /// Хост подключения
    /// </summary>
    public string Host { get; init; } = null!;

    /// <summary>
    /// Распространяемое уведомление об отмене теста
    /// </summary>
    public CancellationToken Cancel { get; set; } = CancellationToken.None;
        
    /// <summary>
    /// Ошибка, случившаяся в ходе выполнения теста
    /// </summary>
    public Exception? Error { get; protected set; }

    protected static string SetData(int i, DataMode mode = DataMode.Int) =>
        mode switch
        {
            DataMode.Guid => Guid.NewGuid().ToString(),
            DataMode.GuidAndInt => $"{Guid.NewGuid()} {i}", 
            _ => i.ToString()
        };
}