using System;
using System.Threading;
using System.Threading.Tasks;
using ServerRequestTestLibrary.SingleRequests;

namespace ServerRequestTestLibrary
{
    /// <summary>
    /// Провайдер управления теста 
    /// </summary>
    public partial class Provider : IDisposable
    {
        #region Private Members
        /// <summary>
        /// Оболочка запросов
        /// </summary>
        private readonly Request _single;

        /// <summary>
        /// Сигнализация отмены/прерывания тестирования
        /// </summary>
        private readonly CancellationTokenSource _cancelSource = new();

        /// <summary>
        /// Получает инфо о сигнале отмены
        /// </summary>
        private bool IsNotCancelled => !_cancelSource.IsCancellationRequested;

        /// <summary>
        /// Запуск теста в консольном приложении
        /// </summary>
        private partial void RunInConsole();

        /// <summary>
        /// приватный счётчик успешно завершённых тестов
        /// </summary>
        private int _successes;
        /// <summary>
        /// Счётчик успешно завершённых тестов
        /// </summary>
        public int Successes => _successes;
        #endregion

        #region Public Members
        /// <summary>
        /// Инициализация провайдера
        /// </summary>
        /// <param name="single">конкретная оболочка единичных тестов</param>
        public Provider(Request single)
        {
            _single = single;
            _single.Cancel = _cancelSource.Token;
        }
        
        /// <summary>
        /// Запуск тестов
        /// </summary>
        /// <returns>Задачу выполнения тестов</returns>
        public Task Run() => Task.Run(() =>
            Parallel.For(0, _single.Count,
                (i, pls) =>
                {
                    if (IsNotCancelled)
                    {
                        try
                        {
                            _single.Run.Invoke(i);
                            Interlocked.Increment(ref _successes);
                        }
                        catch
                        {
                            pls.Break();
                            throw;
                        }
                    }
                    else
                    {
                        pls.Break();
                    }
                }));
        #endregion

        #region Implements of IDisposable
        /// <summary>
        /// Индикатор утилизации
        /// </summary>
        private bool _disposing;

        /// <summary>
        /// Утилизация
        /// </summary>
        public void Dispose()
        {
            Dispose(_disposing);
            GC.SuppressFinalize(this);
            _disposing = true;
        }

        /// <summary>
        /// Утилизация сигнализация отмены тестирования
        /// </summary>
        /// <param name="disposing">Показывает необходимость утилизации</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _cancelSource.Cancel();
                _cancelSource.Dispose();
            }
        }
        #endregion
    }
}
