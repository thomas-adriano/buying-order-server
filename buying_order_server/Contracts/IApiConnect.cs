using System.Threading.Tasks;

namespace buying_order_server.Contracts
{
    public interface IApiConnect
    {
        Task<TResponse> GetDataAsync<TResponse>(string endPoint);
    }
}
