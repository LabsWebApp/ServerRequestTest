using System;
using System.Threading;

namespace ServerRequestTest.SingleRequests
{
    public abstract class Request
    {
        public const int Reserve = 10;

        private ushort _count;
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
        public Action<int> Run { get; init; }
        public string Host { get; init; }
        public CancellationToken Cancel { get; set; } = CancellationToken.None;
        public Exception Error { get; protected set; }
    }
}
