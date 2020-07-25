
using buying_order_server.Data.Entity;
using System.Threading.Tasks;

namespace buying_order_server.Contracts
{
    public interface IAppConfigurationRepository : IRepository<AppConfigurationEntity>
    {
        Task<string> GetCronPattern();
    }
}
