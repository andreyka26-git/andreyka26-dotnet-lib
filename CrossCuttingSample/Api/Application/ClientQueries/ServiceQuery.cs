using Api.Application.ClientModels;
using Api.Application.Shared.CrossCuttingConcerns;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Api.Application.ClientQueries
{
    public class ServiceQueryHandler : IRequestHandler<ServiceQuery, Service>
    {
        private readonly IServicesClient _client;

        public ServiceQueryHandler(IServicesClient client)
        {
            _client = client;
        }

        public Task<Service> Handle(ServiceQuery request, CancellationToken cancellationToken)
        {
            return _client.GetServiceAsync(cancellationToken);
        }
    }

    public class ServiceQuery : IRequest<Service>, IClientQuery
    {
    }
}
