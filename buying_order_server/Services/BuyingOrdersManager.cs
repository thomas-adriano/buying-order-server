

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

        public async Task<List<BuyingOrder>> getBuyingOrdersAsync(CancellationToken cancellationToken)
        {
            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return null;
                }

                var buyingOrders = await _ecosysApi.GetBuyingOrdersAsync(cancellationToken);
                var ordersWithProviderId = buyingOrders.FindAll((order) =>
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

                List<BuyingOrder> ordersAndProviders;

                ordersAndProviders = await Task.Run(async () =>
                {
                    var providers = await _ecosysApi.GetProvidersAsync(cancellationToken);

                    return providers
                        .FindAll(p => p != null && !String.IsNullOrEmpty(p.Email))
                        .ConvertAll(p =>
                        {
                            var order = notPostponedOrders.FindAll(o => o.IdContato == p.Id).FirstOrDefault();
                            return new BuyingOrder { Provider = p, Order = order };
                        });
                }
                , cancellationToken);

                ordersAndProviders = ordersAndProviders.FindAll(o => o.Order != null && o.Provider != null);

                return ordersAndProviders;
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred while trying to create BuyingOrders list. {e.Message}");
            }
            return new List<BuyingOrder>();
        }
    }
}
