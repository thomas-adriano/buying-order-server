
using buying_order_server.Contracts;
using Cronos;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace buying_order_server.Services
{
    public abstract class AbstractCronJob : ICronJob
    {
        protected readonly ILogger<AbstractCronJob> _logger;
        private System.Timers.Timer _timer;
        private CronExpression _expression;
        private string _cronPattern;
        private CancellationTokenSource _startJobCancellationTokenSource;
        private CancellationToken _startJobCancellationToken;
        private IAppExecutionStatusManager _executionStatusManger;

        protected AbstractCronJob(ILogger<AbstractCronJob> logger, IAppExecutionStatusManager executionStatusManger)
        {
            _logger = logger;
            _executionStatusManger = executionStatusManger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _executionStatusManger.ChangeExecutionStatus(DTO.AppExecutionStatuses.Initializing);
            _cronPattern = await CronPattern();
            _startJobCancellationTokenSource = new CancellationTokenSource();
            _startJobCancellationToken = _startJobCancellationTokenSource.Token;
            _logger.LogDebug($"starting Cron Job {_cronPattern}");
            _startJobCancellationToken.Register(async () =>
            {
                _startJobCancellationTokenSource.Cancel();
                _logger.LogInformation("Email sending routine cancelled by the user");
                await StopWork();
                _executionStatusManger.ChangeExecutionStatus(DTO.AppExecutionStatuses.Online);
            });
            await ScheduleJob(_cronPattern, _startJobCancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug($"stopping Cron Job {_cronPattern}");
            _startJobCancellationTokenSource.Cancel();
            _timer?.Stop();

            _executionStatusManger.ChangeExecutionStatus(DTO.AppExecutionStatuses.Online);
        }

        public virtual void Dispose()
        {
            _timer?.Dispose();
        }

        private async Task ScheduleJob(string cronPattern, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return;
            }

            await DoWork(cancellationToken);
            _expression = CronExpression.Parse(cronPattern, CronFormat.IncludeSeconds);
            var next = _expression.GetNextOccurrence(DateTimeOffset.Now, TimeZoneInfo.Local);
            if (next.HasValue)
            {
                var delay = next.Value - DateTimeOffset.Now;
                _timer = new System.Timers.Timer(delay.TotalMilliseconds);
                _timer.Elapsed += async (sender, args) =>
                {
                    try
                    {
                        _timer.Dispose();  // reset and dispose timer
                        _timer = null;
                    }
                    catch (Exception e)
                    {
                        _logger.LogError("Error trying to dispose cron timer");
                    }


                    if (!cancellationToken.IsCancellationRequested)
                    {
                        await DoWork(cancellationToken);
                    }


                    await ScheduleJob(cronPattern, cancellationToken);    // reschedule next

                };
                _timer.Start();
            }
            await Task.CompletedTask;
        }

        protected abstract Task DoWork(CancellationToken cancellationToken);
        protected abstract Task StopWork();
        public abstract Task<string> CronPattern();
    }
}
