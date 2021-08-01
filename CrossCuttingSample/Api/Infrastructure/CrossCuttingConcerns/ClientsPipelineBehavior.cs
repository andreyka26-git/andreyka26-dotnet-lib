using System.Threading;
using System.Threading.Tasks;
using Api.Application.Shared.CrossCuttingConcerns;
using MediatR;

namespace Api.Infrastructure.CrossCuttingConcerns
{
    public class ClientsPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IClientQuery
    {
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var response = await next();
            return response;
        }
    }
}
