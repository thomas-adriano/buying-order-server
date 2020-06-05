using AutoWrapper.Wrappers;
using buying_order_server.DTO.Request;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace buying_order_server.API.v1
{
    [Route("api/v1/order")]
    [ApiController]
    public class OrderController : ControllerBase
    {

        private readonly ILogger<OrderController> _logger;

        public OrderController(ILogger<OrderController> logger)
        {
            _logger = logger;
        }

        [Route("update-date")]
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse), Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), Status422UnprocessableEntity)]
        public ApiResponse updateDate([FromBody] UpdateOrderDateRequest update)
        {
            return new ApiResponse(update);
        }

    }
}