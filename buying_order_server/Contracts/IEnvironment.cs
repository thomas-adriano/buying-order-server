namespace buying_order_server.Contracts
{
    interface IEnvironment
    {

        public bool IsDevelopment();
        public bool IsProduction();
    }
}
