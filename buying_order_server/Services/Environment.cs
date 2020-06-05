
using buying_order_server.Contracts;

namespace buying_order_server.Services
{
    public class Environment : IEnvironment
    {
        private bool _isDevelopment;
        public Environment(bool isDevelopment)
        {
            this._isDevelopment = isDevelopment;
        }

        public bool IsDevelopment()
        {
            return this._isDevelopment;
        }

        public bool IsProduction()
        {
            return !this._isDevelopment;
        }
    }
}
