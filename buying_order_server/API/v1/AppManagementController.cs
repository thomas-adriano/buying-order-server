using AutoWrapper.Wrappers;
using buying_order_server.Contracts;
using buying_order_server.DTO.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace buying_order_server.API.v1
{
    [Route("api/v1/app-management")]
    [ApiController]
    public class AppManagementController : ControllerBase
    {

        private readonly ILogger<AppManagementController> _logger;
        private readonly ICronJob _schedulerService;
        private IAppExecutionStatusManager _executionStatusManager;

        public AppManagementController(ICronJob schedulerService, ILogger<AppManagementController> logger, IAppExecutionStatusManager executionStatusManager)
        {
            _logger = logger;
            _schedulerService = schedulerService;
            _executionStatusManager = executionStatusManager;
        }

        [Route("execution-status")]
        [HttpGet]
        [ProducesResponseType(typeof(ExecutionStatusResponse), Status200OK)]
        public ExecutionStatusResponse GetExecutionStatus()
        {
            var res = new ExecutionStatusResponse();
            res.Status = _executionStatusManager.GetExecutionStatus();
            return res;
        }

        [Route("start-scheduler")]
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse), Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), Status422UnprocessableEntity)]
        public ApiResponse startScheduler(CancellationToken cancellationToken)
        {
            _schedulerService.StartAsync(cancellationToken);
            return new ApiResponse("Email scheduler started");
        }

        [Route("stop-scheduler")]
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse), Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), Status422UnprocessableEntity)]
        public ApiResponse stopScheduler(CancellationToken cancellationToken)
        {
            _schedulerService.StopAsync(cancellationToken);
            return new ApiResponse("Email scheduler stopped");
        }
    }
}