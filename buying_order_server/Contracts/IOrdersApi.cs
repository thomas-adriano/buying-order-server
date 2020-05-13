using buying_order_server.DTO.Response;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace buying_order_server.Contracts
{
    public interface IOrdersApi
    {
        public Task<IEnumerable<BuyingOrdersResponse>> GetBuyingOrdersAsync();
        public Task<ProviderResponse> GetProviderByIdAsync(string providerId);
    }
}
