namespace ServerRequestTestLibrary.SingleRequests.Helper;

public class ExceptionComparer : IEqualityComparer<Exception>
{
    public bool Equals(Exception? x, Exception? y) => (x, y) switch
    {
        (null, null) => true,
        (null, _) => false,
        (_, null) => false,
        _ => x.HResult == y.HResult && x.Message == y.Message &&
             Equals(x.InnerException, y.InnerException)
    };

    public int GetHashCode(Exception? obj) => obj switch
    {
        null => 0,
        _ => obj.HResult.GetHashCode() ^ obj.Message.GetHashCode() ^ GetHashCode(obj.InnerException)
    };
}