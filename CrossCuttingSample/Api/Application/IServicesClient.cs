using System.Threading;
using Api.Application.ClientModels;
using System.Threading.Tasks;

namespace Api.Application
{
    public interface IServicesClient
    {
        Task<Service> GetServiceAsync(CancellationToken cancellationToken);
        Task<ServiceAdditionalInfo> GetServiceAdditionalInfoAsync(CancellationToken cancellationToken);
    }
}
