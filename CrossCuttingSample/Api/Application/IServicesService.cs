using System.Threading;
using System.Threading.Tasks;
using Api.Application.ServiceModels;

namespace Api.Application
{
    public interface IServicesService
    {
        Task<InternalService> GetServiceAsync(CancellationToken cancellationToken);
    }
}
