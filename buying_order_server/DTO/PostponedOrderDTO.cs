
using FluentValidation;
using System;

namespace buying_order_server.DTO.Request
{
    public class PostponedOrderDTO : DTO
    {
        public string OrderId { get; set; }
        public DateTime Date { get; set; }
        public int Count { get; set; }
    }

    public class PostponeOrderRequestValidator : AbstractValidator<PostponedOrderDTO>
    {
        public PostponeOrderRequestValidator()
        {
            RuleFor(o => o.OrderId).NotEmpty();
            RuleFor(o => o.Date).NotEmpty();

        }
    }
}
