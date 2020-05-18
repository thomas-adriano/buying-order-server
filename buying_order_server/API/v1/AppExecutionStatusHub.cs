using buying_order_server.Contracts;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace buying_order_server.API.v1
{
    public class AppExecutionStatusHub : Hub
    {

        private IAppExecutionStatusManager _appExecutionStatusManager;
        private ILogger<AppExecutionStatusHub> _logger;

        public AppExecutionStatusHub(IAppExecutionStatusManager appExecutionStatusManager, ILogger<AppExecutionStatusHub> logger)
        {
            _appExecutionStatusManager = appExecutionStatusManager;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            var lastExecutionStatus = _appExecutionStatusManager.GetExecutionStatus();
            if (lastExecutionStatus != 0)
            {
                try
                {
                    await Clients.All.SendAsync("app-execution-status-changed", lastExecutionStatus);
                }
                catch (Exception e)
                {
                    _logger.LogError("Could not send hub message", e);
                }
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
            _logger.LogInformation("Status Hub connection disconnected");
        }

        protected override void Dispose(bool disposing)
        {
            _logger.LogInformation("Status Hub Disposed");
            base.Dispose(disposing);
        }


    }
}
