﻿

using System;

namespace buying_order_server.Data.Entity
{
    public class PostponedOrder
    {
        public long OrderId { get; set; }
        public DateTime Date { get; set; }
        public int Count { get; set; }
    }
}
