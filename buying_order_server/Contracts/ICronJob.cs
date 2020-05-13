
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace buying_order_server.Contracts
{
    public interface ICronJob : IHostedService, IDisposable
    {
        public Task<string> CronPattern();
    }
}
