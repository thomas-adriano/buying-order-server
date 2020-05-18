

using buying_order_server.Contracts;
using buying_order_server.DTO;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        public async Task<IEnumerable<BuyingOrder>> getBuyingOrdersAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return null;
            }

            var buyingOrders = await _ecosysApi.GetBuyingOrdersAsync(cancellationToken);
            var ordersWithProviderId = buyingOrders.Where((order) =>
            {
                return !String.IsNullOrEmpty(order.IdContato);
            });

            IEnumerable<BuyingOrder> ordersAndProviders;

            ordersAndProviders = await Task.Run(async () =>
            {
                var providers = await Task.WhenAll(
                ordersWithProviderId
                    .Select(e => _ecosysApi.GetProviderByIdAsync(e.IdContato, cancellationToken))
                    .Where(e => e != null)
                );

                return providers
                .Where(o => o != null && !String.IsNullOrEmpty(o.Email))
                .Select(p =>
                {
                    var order = ordersWithProviderId.Where(o => o.IdContato == p.Id).First();
                    return new BuyingOrder { Provider = p, Order = ordersWithProviderId.Where(o => o.IdContato == p.Id).First() };
                });
            }
            , cancellationToken);

            return ordersAndProviders;
        }
    }
}
