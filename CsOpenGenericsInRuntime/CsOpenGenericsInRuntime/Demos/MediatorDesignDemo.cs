using CsOpenGenericsInRuntime.Services;

namespace CsOpenGenericsInRuntime.Demos
{
    //inspired by MediatR
    //https://github.com/jbogard/MediatR/blob/340381e0ea/src/MediatR/Wrappers/RequestHandlerWrapper.cs
    public class MediatorDesignDemo
    {
        private readonly IServiceProvider _serviceProvider;
        public MediatorDesignDemo(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task RunAsync()
        {
            var firstReq = new FirstServiceRequest();
            var handler = GetHandler(firstReq);

            await handler.HandleAsync(firstReq, _serviceProvider);

            var secondReq = new SecondServiceRequest();
            handler = GetHandler(secondReq);

            await handler.HandleAsync(secondReq, _serviceProvider);
        }

        public RequestHandlerBase GetHandler(IServiceRequest req)
        {
            var type = typeof(Handler<>);
            var implType = type.MakeGenericType(req.GetType());

            var handler = (RequestHandlerBase)Activator.CreateInstance(implType);
            return handler;
        }
    }

    public abstract class RequestHandlerBase
    {
        public abstract Task HandleAsync(object request, IServiceProvider serviceProvider);
        public abstract Task HandleAsync(IServiceRequest request, IServiceProvider serviceProvider);
    }

    //should have parameterless constructor
    public class Handler<TReq> : RequestHandlerBase
        where TReq : class, IServiceRequest
    {
        public override async Task HandleAsync(object request, IServiceProvider serviceProvider)
        {
            await HandleAsync((IServiceRequest)request, serviceProvider);
        }

        public override async Task HandleAsync(IServiceRequest request, IServiceProvider serviceProvider)
        {
            var service = serviceProvider.GetRequiredService<IService<TReq>>();

            await service.HandleAsync((TReq)request);
        }
    }
}
