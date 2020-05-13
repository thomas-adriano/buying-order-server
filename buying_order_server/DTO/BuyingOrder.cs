

using buying_order_server.DTO.Response;

namespace buying_order_server.DTO
{
    public class BuyingOrder
    {
        public BuyingOrdersResponse Order { get; set; }
        public ProviderResponse Provider { get; set; }
    }
}
