﻿using buying_order_server.DTO.Response;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace buying_order_server.Contracts
{
    public interface IOrdersApi
    {
        public Task<List<BuyingOrdersDTO>> GetBuyingOrdersAsync(CancellationToken cancellationToken);
        public Task<List<ProviderDTO>> GetProvidersAsync(CancellationToken cancellationToken);
    }
}
