using System.Threading;
using Api.Application.ServiceModels;
using System.Threading.Tasks;
using Api.Application;
using Api.Application.ClientModels;
using Api.Application.ClientQueries;
using MediatR;

namespace Api.Infrastructure
{
    public class ServicesService : IServicesService
    {
        private readonly IMediator _mediator;

        public ServicesService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<InternalService> GetServiceAsync(CancellationToken cancellationToken)
        {
            var serviceQuery = new ServiceQuery();
            var service = await _mediator.Send(serviceQuery, cancellationToken);

            var addInfoQuery = new AdditionalInfoQuery();
            var additionalInfo = await _mediator.Send(addInfoQuery, cancellationToken);

            var response = new InternalService
            {
                AdditionalInfo = additionalInfo.AdditionalInfo,
                ServiceName = service.ServiceName,
                Url = service.Url
            };

            return response;
        }

        public async Task<ServiceAdditionalInfo> GetServiceAdditionalInfo(CancellationToken cancellationToken)
        {
            var addInfoQuery = new AdditionalInfoQuery();
            var additionalInfo = await _mediator.Send(addInfoQuery, cancellationToken);
            return additionalInfo;
        }
    }
}
