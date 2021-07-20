using System.Threading;
using Api.Application.ServiceModels;
using System.Threading.Tasks;
using Api.Application;

namespace Api.Infrastructure
{
    public class ServicesService : IServicesService
    {
        private readonly IServicesClient _client;

        public ServicesService(IServicesClient client)
        {
            _client = client;
        }

        public async Task<InternalService> GetServiceAsync(CancellationToken cancellationToken)
        {
            var service = await _client.GetServiceAsync(cancellationToken);
            var additionalInfo = await _client.GetServiceAdditionalInfoAsync(cancellationToken);

            var response = new InternalService
            {
                AdditionalInfo = additionalInfo.AdditionalInfo,
                ServiceName = service.ServiceName,
                Url = service.Url
            };

            return response;
        }
    }
}
