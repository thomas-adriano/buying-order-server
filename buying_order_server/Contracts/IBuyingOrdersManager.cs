
using buying_order_server.Data.Entity;
using buying_order_server.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace buying_order_server.Contracts
{
    public interface IBuyingOrdersManager
    {
        public Task<IEnumerable<BuyingOrder>> getBuyingOrdersAsync();
    }
}
