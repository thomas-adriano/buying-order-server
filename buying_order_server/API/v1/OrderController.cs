using AutoMapper;
using AutoWrapper.Wrappers;
using buying_order_server.Contracts;
using buying_order_server.Data.Entity;
using buying_order_server.DTO.Request;
using buying_order_server.DTO.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace buying_order_server.API.v1
{
    [Route("api/v1/order")]
    [ApiController]
    public class OrderController : ControllerBase
    {

        private readonly ILogger<OrderController> _logger;
        private IPostponedOrderRepository _repo;
        private IMapper _mapper;

        public OrderController(ILogger<OrderController> logger, IPostponedOrderRepository repo, IMapper mapper)
        {
            _logger = logger;
            _repo = repo;
            _mapper = mapper;
        }

        [Route("update-date")]
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse), Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), Status422UnprocessableEntity)]
        public async Task<ApiResponse> updateDate([FromBody] PostponedOrderDTO update)
        {
            var order = await _repo.CreateOrUpdateAsync(_mapper.Map<PostponedOrderEntity>(update));
            return new ApiResponse(_mapper.Map<PostponedOrderDTO>(order));
        }

    }
}