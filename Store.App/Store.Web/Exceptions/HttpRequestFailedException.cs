namespace Store.Web.Exceptions
{
    public class HttpRequestFailedException : Exception
    {
        public HttpRequestFailedException() { }

        public HttpRequestFailedException(string message)
        : base(message)
        {
        }

        public HttpRequestFailedException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
