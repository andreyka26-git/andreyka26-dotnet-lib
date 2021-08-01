using System.Threading;
using System.Threading.Tasks;
using Api.Application.ClientModels;
using Api.Application.Shared.CrossCuttingConcerns;
using MediatR;

namespace Api.Application.ClientQueries
{
    public class AdditionalInfoQueryHandler : IRequestHandler<AdditionalInfoQuery, ServiceAdditionalInfo>
    {
        private readonly IServicesClient _client;

        public AdditionalInfoQueryHandler(IServicesClient client)
        {
            _client = client;
        }
        
        public Task<ServiceAdditionalInfo> Handle(AdditionalInfoQuery request, CancellationToken cancellationToken)
        {
            return _client.GetServiceAdditionalInfoAsync(cancellationToken);
        }
    }
    public class AdditionalInfoQuery : IRequest<ServiceAdditionalInfo>, IAdditionalQuery
    {
    }
}
