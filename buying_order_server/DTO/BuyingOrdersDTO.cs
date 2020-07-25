
using System;

namespace buying_order_server.DTO.Response
{
    public class BuyingOrdersDTO : DTO
    {
        public string NumeroPedido { get; set; }
        public string DataPrevista { get; set; }
        public string Data { get; set; }
        public string IdContato { get; set; }
        public string NomeContato { get; set; }

    }
}
