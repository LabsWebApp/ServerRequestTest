using System;
using System.Threading;
using System.Threading.Tasks;
using ServerRequestTest.SingleRequests;

namespace ServerRequestTest
{
    public partial class Provider : IDisposable
    {
        #region Private Members
        private readonly Request _single;
        private readonly CancellationTokenSource _cancelSource = new();
        private partial void RunInConsole();
        private bool IsNotCancelled => !_cancelSource.IsCancellationRequested;

        private int _successes;
        public int Successes => _successes;
        #endregion

        #region Public Members
        public Provider(Request single)
        {
            _single = single;
            _single.Cancel = _cancelSource.Token;
        }
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
        #endregion
    }
}
