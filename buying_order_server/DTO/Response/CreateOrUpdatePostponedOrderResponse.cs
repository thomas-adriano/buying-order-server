using System;

namespace buying_order_server.DTO.Response
{
    public class CreateOrUpdatePostponedOrderResponse
    {
        public string OrderId { get; set; }
        public DateTime Date { get; set; }
        public int Count { get; set; }
    }
}
