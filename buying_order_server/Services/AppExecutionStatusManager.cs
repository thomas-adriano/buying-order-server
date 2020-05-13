

using buying_order_server.Contracts;
using buying_order_server.DTO;
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

        public AppExecutionStatusManager(IHostApplicationLifetime hostApplicationLifetime, ILogger<AppExecutionStatusManager> logger)
        {
            _logger = logger;

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
    }
}
