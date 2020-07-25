

using buying_order_server.DTO.Response;

namespace buying_order_server.DTO
{
    public class BuyingOrderWithProviderDTO : DTO
    {
        public BuyingOrdersDTO Order { get; set; }
        public ProviderDTO Provider { get; set; }
    }
}
