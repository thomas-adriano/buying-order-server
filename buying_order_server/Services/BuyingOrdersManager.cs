

using buying_order_server.Contracts;
using buying_order_server.DTO;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace buying_order_server.Services
{


    public class BuyingOrdersManager : IBuyingOrdersManager
    {
        private ILogger _logger;
        private IOrdersApi _ecosysApi;

        public BuyingOrdersManager(ILogger<BuyingOrdersManager> logger, IOrdersApi ecosysApi)
        {
            _logger = logger;
            _ecosysApi = ecosysApi;
        }

        public async Task<IEnumerable<BuyingOrder>> getBuyingOrdersAsync()
        {
            var buyingOrders = await _ecosysApi.GetBuyingOrdersAsync();
            var ordersWithProviderId = buyingOrders.Where((order) =>
            {
                return !String.IsNullOrEmpty(order.IdContato);
            });

            var providers = await Task.WhenAll(
                ordersWithProviderId
                    .Select(e => _ecosysApi.GetProviderByIdAsync(e.IdContato))
                    .Where(e => e != null)
                );
            var ordersAndProviders = providers
                .Where(o => o != null && !String.IsNullOrEmpty(o.Email))
                .Select(p =>
                {
                    var order = ordersWithProviderId.Where(o => o.IdContato == p.Id).First();
                    return new BuyingOrder { Provider = p, Order = ordersWithProviderId.Where(o => o.IdContato == p.Id).First() };
                });
            return ordersAndProviders;
        }
    }
}
