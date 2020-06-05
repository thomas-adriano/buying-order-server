
using FluentValidation;

namespace buying_order_server.DTO.Request
{
    public class UpdateOrderDateRequest
    {
        public string OrderId { get; set; };
        public string Date { get; set; }
    }

    public class UpdateOrderDateRequestValidator : AbstractValidator<UpdateOrderDateRequest>
    {
        public UpdateOrderDateRequestValidator()
        {
            RuleFor(o => o.OrderId).NotEmpty();
            RuleFor(o => o.Date).NotEmpty();

        }
    }
}
