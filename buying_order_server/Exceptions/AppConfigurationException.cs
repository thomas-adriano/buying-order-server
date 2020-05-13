using System;

namespace buying_order_server.Exceptions
{
    public class AppConfigurationException : Exception
    {
        public AppConfigurationException()
        {
        }

        public AppConfigurationException(string message)
            : base(message)
        {
        }

        public AppConfigurationException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
