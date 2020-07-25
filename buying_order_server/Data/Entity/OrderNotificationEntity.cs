using System;

namespace buying_order_server.Data.Entity
{
    public class OrderNotificationEntity : EntityBaseEntity
    {
        public long BuyingOrderId { get; set; }
        public long ProviderId { get; set; }
        public DateTime Timestamp { get; set; }
        public bool Send { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime EstimatedOrderDate { get; set; }
        public string ProviderEmail { get; set; }
        public string EmployeeEmail { get; set; }
    }
}
