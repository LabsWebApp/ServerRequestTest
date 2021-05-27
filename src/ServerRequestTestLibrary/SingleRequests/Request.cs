using System;
using System.Threading;

namespace ServerRequestTestLibrary.SingleRequests
{
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
                //ThreadPool.GetMaxThreads(out int maxWorkerThreads, out int _);
                ThreadPool.GetAvailableThreads(out var workerThreads, out var _);
                var setter = Math.Min(workerThreads, ushort.MaxValue);
                if (value > setter)
                    _count = (ushort)(setter - Reserve);
                else
                    _count = (ushort)value;
            }
        }
        
        /// <summary>
        /// Инкапсуляция запуска единичного теста
        /// </summary>
        public Action<int> Run { get; init; }

        /// <summary>
        /// Хост подключения
        /// </summary>
        public string Host { get; init; }

        /// <summary>
        /// Распространяемое уведомление об отмене теста
        /// </summary>
        public CancellationToken Cancel { get; set; } = CancellationToken.None;
        
        /// <summary>
        /// Ошибка, случившаяся в ходе выполнения теста
        /// </summary>
        public Exception Error { get; protected set; }
    }
}
