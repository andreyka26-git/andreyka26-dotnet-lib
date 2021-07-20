using Api.Application.ClientModels;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Api.Application;
using Newtonsoft.Json;

namespace Api.Infrastructure
{
    public class ServicesClient : IServicesClient
    {
        private readonly string RootRoute = "http://localhost:5001";
        private readonly HttpClient _httpClient;

        public ServicesClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Service> GetServiceAsync(CancellationToken cancellationToken)
        {
            using (var response = await _httpClient.GetAsync($"{RootRoute}/service", cancellationToken))
            {
                var json = await response.Content.ReadAsStringAsync();
                var service = JsonConvert.DeserializeObject<Service>(json);
                return service;
            }
        }

        public async Task<ServiceAdditionalInfo> GetServiceAdditionalInfoAsync(CancellationToken cancellationToken)
        {
            using (var response = await _httpClient.GetAsync($"{RootRoute}/service/additional-info", cancellationToken))
            {
                var json = await response.Content.ReadAsStringAsync();
                var additionalInfo = JsonConvert.DeserializeObject<ServiceAdditionalInfo>(json);
                return additionalInfo;
            }
        }
    }
}
