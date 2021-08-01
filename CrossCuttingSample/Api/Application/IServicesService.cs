using System.Threading;
using System.Threading.Tasks;
using Api.Application.ClientModels;
using Api.Application.ServiceModels;

namespace Api.Application
{
    public interface IServicesService
    {
        Task<InternalService> GetServiceAsync(CancellationToken cancellationToken);
        Task<ServiceAdditionalInfo> GetServiceAdditionalInfo(CancellationToken cancellationToken);
    }
}
