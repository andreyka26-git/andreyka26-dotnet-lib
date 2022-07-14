using CsOpenGenericsInRuntime.Services;

namespace CsOpenGenericsInRuntime.Demos
{
    //inspired by MediatR
    //https://github.com/jbogard/MediatR/blob/340381e0ea/src/MediatR/Wrappers/RequestHandlerWrapper.cs
    public class MediatorDesignDemo : DemoBase
    {
        private readonly IServiceProvider _serviceProvider;
        public MediatorDesignDemo(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override async Task RunAsync()
        {
            var firstReq = GetFirstRequest();
            var handler = GetHandler(firstReq);

            //TryGetServiceHandler(firstReq);

            await handler.HandleAsync(firstReq, _serviceProvider);

            var secondReq = GetSecondRequest();
            handler = GetHandler(secondReq);

            await handler.HandleAsync(secondReq, _serviceProvider);
        }

        //public IService<T> TryGetServiceHandler<T>(T req)
        //    where T : class, IServiceRequest
        //{
        //    var reqType = req.GetType();
        //    var genType = typeof(IService<>);
        //    var type = genType.MakeGenericType(reqType);

        //    var service = _serviceProvider.GetRequiredService(type);

        //    //This will fail
        //    return (IService<T>)service;
        //}

        public IRequestHandlerBase GetHandler(IServiceRequest req)
        {
            var type = typeof(Handler<>);
            var implType = type.MakeGenericType(req.GetType());

            var handler = (IRequestHandlerBase)Activator.CreateInstance(implType);
            return handler;
        }
    }

    public interface IRequestHandlerBase
    {
        public Task HandleAsync(IServiceRequest request, IServiceProvider serviceProvider);
    }

    //should have parameterless constructor because of Activator
    public class Handler<TReq> : IRequestHandlerBase
        where TReq : class, IServiceRequest
    {
        public async Task HandleAsync(IServiceRequest request, IServiceProvider serviceProvider)
        {
            var service = serviceProvider.GetRequiredService<IService<TReq>>();

            await service.HandleAsync((TReq)request);
        }
    }
}
