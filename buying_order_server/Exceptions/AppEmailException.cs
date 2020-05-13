using System;

namespace buying_order_server.Exceptions
{
    public class AppEmailException : Exception
    {
        public AppEmailException()
        {
        }

        public AppEmailException(string message)
            : base(message)
        {
        }

        public AppEmailException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
