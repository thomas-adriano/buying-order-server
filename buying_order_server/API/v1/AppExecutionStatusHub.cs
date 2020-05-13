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
        private IDisposable _executionStatusChangedSubscription;

        public AppExecutionStatusHub(IAppExecutionStatusManager appExecutionStatusManager, ILogger<AppExecutionStatusHub> logger)
        {
            _appExecutionStatusManager = appExecutionStatusManager;
            _logger = logger;
            _logger.LogInformation("@@@@@@@@@@@@@@@@@@@@@@@@@@@@ @@@@@@@@@@@@@@@@@@@@@@@@ @@@@@@@@@@@");
            if (_executionStatusChangedSubscription != null)
            {
                _logger.LogInformation("Execution status subscriber initialized.");
                _executionStatusChangedSubscription = _appExecutionStatusManager.ExecutionStatusChanged().Subscribe(
                 s =>
                 {
                     _logger.LogInformation("Execution status hub message being send");
                     try
                     {
                         Clients?.All?.SendAsync("app-execution-status-changed", s);
                     }
                     catch (Exception e)
                     {
                         _logger.LogError("Could not send hub message", e);
                     }
                 },
                 err =>
                 {
                     _logger.LogError("An error ocurred trying to send status changed singalr");
                 }
             );
            }
        }

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation("@@@@@@@@@@@@@@@@@@@@@@@@@@@@ @@@@@@@@@@@@@@@@@@@@@@@@ @@@@@@@@@@@");
            await base.OnConnectedAsync();
            _logger.LogInformation("@@@@@@@@@@@@@@@@@@@@@@@@@@@@ @@@@@@@@@@@@@@@@@@@@@@@@ @@@@@@@@@@@@@@@Status Hub connection stabilished");
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
            _executionStatusChangedSubscription?.Dispose();
        }


    }
}
