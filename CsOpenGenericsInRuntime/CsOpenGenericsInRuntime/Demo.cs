using CsOpenGenericsInRuntime.Services;
using System.Reflection;

namespace CsOpenGenericsInRuntime
{
    public class Demo
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Type[] _types;
        public Demo(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;

            _types = Assembly.GetExecutingAssembly().GetTypes();
        }

        public async Task RunAsync()
        {
            var firstReq = new FirstServiceRequest();

            var service = GetServiceImplementation(firstReq);
            service.HandleAsync(firstReq);

            var secondReq = new SecondServiceRequest();

            service = GetServiceImplementation(secondReq);
            service.HandleAsync(secondReq);
        }

        public dynamic GetServiceImplementation(IServiceRequest req)
        {
            var reqName = req.RequestName;
            var type = _types.Where(n => n.Name.Contains(reqName)).SingleOrDefault();

            var gen = typeof(IService<>);
            var serviceType = gen.MakeGenericType(type);

            var service = _serviceProvider.GetService(serviceType);
            return service;
        }
    }
}
