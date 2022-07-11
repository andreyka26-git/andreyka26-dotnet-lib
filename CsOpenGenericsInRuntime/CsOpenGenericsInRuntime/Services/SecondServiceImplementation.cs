namespace CsOpenGenericsInRuntime.Services
{
    public class SecondServiceRequest : IServiceRequest
    {
        public string RequestName => nameof(SecondServiceRequest);
        public string MyProperty { get; set; } = "MyProperty";
    }

    public class SecondServiceImplementation : IService<SecondServiceRequest>
    {
        public Task HandleAsync(SecondServiceRequest request)
        {
            return Task.CompletedTask;
        }
    }
}
