using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using buying_order_server.DTO.Response;
using System.Collections.Generic;
using System.Text.Json;
using buying_order_server.Contracts;
using System;

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

        public async Task<IEnumerable<BuyingOrdersResponse>> GetBuyingOrdersAsync()
        {
            var httpResponse = await _httpClient.GetAsync("/api/ordens-de-compra?situacoes=0");

            if (!httpResponse.IsSuccessStatusCode)
            {
                _logger.Log(LogLevel.Warning, $"[{httpResponse.StatusCode}] An error occured while requesting external api.");
                return default;
            }

            var jsonString = await httpResponse.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<List<BuyingOrdersResponse>>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            return data;
        }

        public async Task<ProviderResponse> GetProviderByIdAsync(string providerId)
        {
            try
            {
                var httpResponse = await _httpClient.GetAsync($"/api/fornecedores/{providerId}");
                if (!httpResponse.IsSuccessStatusCode)
                {
                    _logger.Log(LogLevel.Warning, $"[{httpResponse.StatusCode}] An error occured while requesting external api.");
                    return default;
                }

                var jsonString = await httpResponse.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<List<ProviderResponse>>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return data[0];
            }
            catch (Exception e)
            {
                _logger.LogError($"An error occurred trying to fetch provider {providerId}");
                return null;
            }

        }

    }
}
