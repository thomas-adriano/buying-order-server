

using buying_order_server.DTO;
using System;

namespace buying_order_server.Contracts
{
    public interface IAppExecutionStatusManager: IDisposable
    {
        public AppExecutionStatuses GetExecutionStatus();

        public void ChangeExecutionStatus(AppExecutionStatuses status);

        public IObservable<AppExecutionStatuses> ExecutionStatusChanged();
    }
}
