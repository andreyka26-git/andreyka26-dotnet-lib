using System.Threading;
using System.Threading.Tasks;
using Api.Application.Shared.CrossCuttingConcerns;
using MediatR;

namespace Api.Infrastructure.CrossCuttingConcerns
{
    public class AdditionalPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IAdditionalQuery
    {
        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            return next();
        }
    }
}
