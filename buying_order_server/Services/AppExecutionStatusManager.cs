

using buying_order_server.API.v1;
using buying_order_server.Contracts;
using buying_order_server.DTO;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace buying_order_server.Services
{
    public class AppExecutionStatusManager : IAppExecutionStatusManager
    {
        private ILogger<AppExecutionStatusManager> _logger;
        private BehaviorSubject<AppExecutionStatuses> _executionStatusSubject = new BehaviorSubject<AppExecutionStatuses>(AppExecutionStatuses.Null);
        private IDisposable _executionStatusChangedSubscription;
        private IHubContext<AppExecutionStatusHub> _appStatusHubContext;

        public AppExecutionStatusManager(IHostApplicationLifetime hostApplicationLifetime, ILogger<AppExecutionStatusManager> logger, IHubContext<AppExecutionStatusHub> appStatusHubContext)
        {
            _logger = logger;
            _appStatusHubContext = appStatusHubContext;


            _logger.LogInformation("Execution status subscriber initialized.");
            _executionStatusChangedSubscription = ExecutionStatusChanged().Subscribe(
             s =>
                 {
                     _logger.LogInformation("Execution status hub message being send");
                     try
                     {
                         _appStatusHubContext?.Clients?.All?.SendAsync("app-execution-status-changed", s);
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



            if (hostApplicationLifetime.ApplicationStarted.IsCancellationRequested)
            {
                ChangeExecutionStatus(AppExecutionStatuses.Online);
            }
            hostApplicationLifetime.ApplicationStarted.Register(() =>
            {
                ChangeExecutionStatus(AppExecutionStatuses.Online);
            });

            hostApplicationLifetime.ApplicationStopping.Register(() =>
            {
                ChangeExecutionStatus(AppExecutionStatuses.Finalizing);
            });

            hostApplicationLifetime.ApplicationStopped.Register(() =>
            {
                ChangeExecutionStatus(AppExecutionStatuses.Offline);
            });
        }

        public AppExecutionStatuses GetExecutionStatus()
        {
            return _executionStatusSubject.Value;
        }

        public void ChangeExecutionStatus(AppExecutionStatuses status)
        {
            _logger.LogInformation($"Execution status changed {status}");
            _executionStatusSubject.OnNext(status);
        }

        public IObservable<AppExecutionStatuses> ExecutionStatusChanged()
        {
            return _executionStatusSubject.AsObservable();
        }

        public void Dispose()
        {
            _executionStatusChangedSubscription?.Dispose();
        }

    }
}
