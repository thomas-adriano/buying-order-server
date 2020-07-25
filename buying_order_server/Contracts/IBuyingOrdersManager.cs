using buying_order_server.DTO;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace buying_order_server.Contracts
{
    public interface IBuyingOrdersManager
    {
        public Task<List<BuyingOrderWithProviderDTO>> getBuyingOrdersAsync(CancellationToken cancellationToken);
    }
}
