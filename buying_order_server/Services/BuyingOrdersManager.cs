

using buying_order_server.Contracts;
using buying_order_server.DTO;
using buying_order_server.DTO.Response;
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
        private IPostponedOrderRepository _postponedOrderRepo;

        public BuyingOrdersManager(ILogger<BuyingOrdersManager> logger, IOrdersApi ecosysApi, IPostponedOrderRepository postponedOrderRepo)
        {
            _logger = logger;
            _ecosysApi = ecosysApi;
            _postponedOrderRepo = postponedOrderRepo;
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

            var notPostponedOrders = new List<BuyingOrdersResponse>();
            foreach (BuyingOrdersResponse o in ordersWithProviderId)
            {
                if (o == null)
                {
                    continue;
                }
                var postponed = await _postponedOrderRepo.GetByIdAsync(o.NumeroPedido);
                if (postponed == null)
                {
                    notPostponedOrders.Add(o);
                }
                else
                {
                    var later = DateTime.Compare(DateTime.Now, postponed.Date) > 0;
                    if (later)
                    {
                        notPostponedOrders.Add(o);
                    }
                }
            }

            IEnumerable<BuyingOrder> ordersAndProviders;

            ordersAndProviders = await Task.Run(async () =>
            {
                var providers = await Task.WhenAll(
                notPostponedOrders
                    .Select(e => _ecosysApi.GetProviderByIdAsync(e.IdContato, cancellationToken))
                    .Where(e => e != null)
                );

                return providers
                .Where(o => o != null && !String.IsNullOrEmpty(o.Email))
                .Select(p =>
                {
                    var order = notPostponedOrders.Where(o => o.IdContato == p.Id).First();
                    return new BuyingOrder { Provider = p, Order = notPostponedOrders.Where(o => o.IdContato == p.Id).First() };
                });
            }
            , cancellationToken);

            return ordersAndProviders;
        }
    }
}
