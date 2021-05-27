﻿using System;
using System.Threading;
using System.Threading.Tasks;
using ServerRequestTestLibrary.SingleRequests;

namespace ServerRequestTestLibrary
{
    /// <summary>
    /// Провайдер управления теста 
    /// </summary>
    public class Provider : IDisposable
    {
        #region Protected Members
        /// <summary>
        /// Сигнализация отмены/прерывания тестирования
        /// </summary>
        protected readonly CancellationTokenSource CancelSource = new();

        /// <summary>
        /// Получает инфо о сигнале отмены
        /// </summary>
        protected bool IsNotCancelled => !CancelSource.IsCancellationRequested;

        /// <summary>
        /// Кол-во параллельных тестов
        /// </summary>
        protected int Count => _single.Count;
        #endregion

        #region Private Members
        /// <summary>
        /// Оболочка запросов
        /// </summary>
        private readonly Request _single;

        /// <summary>
        /// приватный счётчик успешно завершённых тестов
        /// </summary>
        private int _successes;
        #endregion

        #region Public Members
        /// <summary>
        /// Счётчик успешно завершённых тестов
        /// </summary>
        public int Successes => _successes;

        /// <summary>
        /// Инициализация провайдера
        /// </summary>
        /// <param name="single">конкретная оболочка единичных тестов</param>
        public Provider(Request single)
        {
            _single = single;
            _single.Cancel = CancelSource.Token;
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
        /// Утилизация
        /// </summary>
        public void Dispose()
        {
            CancelSource.Cancel();
            CancelSource.Dispose();
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}