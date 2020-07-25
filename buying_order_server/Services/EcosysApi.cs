using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using buying_order_server.DTO.Response;
using System.Collections.Generic;
using System.Text.Json;
using buying_order_server.Contracts;
using System;
using System.Threading;

namespace buying_order_server.Services
{
    public class EcosysApi : IOrdersApi
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<EcosysApi> _logger;
        public EcosysApi(HttpClient httpClient, ILogger<EcosysApi> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<BuyingOrdersDTO>> GetBuyingOrdersAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return null;
            }
            var endpoint = "/api/ordens-de-compra?situacoes=0";
            var httpResponse = await _httpClient.GetAsync(endpoint, cancellationToken);

            if (!httpResponse.IsSuccessStatusCode)
            {
                _logger.Log(LogLevel.Warning, $"[{httpResponse.StatusCode}] An error occured while requesting external api {endpoint}");
                return default;
            }

            var jsonString = await httpResponse.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<List<BuyingOrdersDTO>>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return data;
        }

        public async Task<List<ProviderDTO>> GetProvidersAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return null;
            }

            try
            {
                var endpoint = $"/api/fornecedores";
                var httpResponse = await _httpClient.GetAsync(endpoint, cancellationToken);

                if (!httpResponse.IsSuccessStatusCode)
                {
                    _logger.Log(LogLevel.Warning, $"[{httpResponse.StatusCode}] An error occured while requesting external api {endpoint}");
                    return default;
                }

                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<List<ProviderDTO>>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return data;
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred trying to fetch providers. {e.Message}\n {e.StackTrace}");
                return null;
            }

        }

    }
}
