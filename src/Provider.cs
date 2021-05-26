using System;
using System.Threading;
using System.Threading.Tasks;
using ServerRequestTest.SingleRequests;
using ServerRequestTest.SingleRequests.Exceptions;

namespace ServerRequestTest
{
    public partial class Provider : IDisposable
    {
        private readonly Request _single;
        private readonly CancellationTokenSource _cancelSource = new();
        private readonly object _lock = new();
        private partial void RunInConsole();
        private bool IsNotCancelled => !_cancelSource.IsCancellationRequested;

        public ushort Successes { get; private set; }

        public Provider(Request single)
        {
            _single = single;
            _single.Cancel = _cancelSource.Token;
        } 

        public Task RunAsync() => Task.Run(() =>
            Parallel.For(0, _single.Count, 
                (i, pls) =>
            {
                if (IsNotCancelled)
                {
                    try
                    {
                        _single.Run.Invoke(i);
                        lock (_lock)
                        {
                            Successes++;
                        }
                    }
                    catch 
                    {
                        _cancelSource.Cancel();
                        pls.Break();
                        throw;
                    }
                }
                else
                {
                    _cancelSource.Cancel();
                    pls.Break();
                }
            }));

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _cancelSource.Cancel();
                _cancelSource.Dispose();
            }
        }
    }
}
