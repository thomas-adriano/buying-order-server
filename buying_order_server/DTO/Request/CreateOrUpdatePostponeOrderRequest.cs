
using FluentValidation;
using System;

namespace buying_order_server.DTO.Request
{
    public class CreateOrUpdatePostponeOrderRequest
    {
        public string OrderId { get; set; }
        public DateTime Date { get; set; }
    }

    public class PostponeOrderRequestValidator : AbstractValidator<CreateOrUpdatePostponeOrderRequest>
    {
        public PostponeOrderRequestValidator()
        {
            RuleFor(o => o.OrderId).NotEmpty();
            RuleFor(o => o.Date).NotEmpty();

        }
    }
}
