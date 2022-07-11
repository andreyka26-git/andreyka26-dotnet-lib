namespace CsOpenGenericsInRuntime.Services
{
    public class FirstServiceRequest : IServiceRequest
    {
        public string RequestName => nameof(FirstServiceRequest);
        public string AnotherProperty { get; set; } = "Another";
    }

    public class FirstServiceImplementation : IService<FirstServiceRequest>
    {
        public Task HandleAsync(FirstServiceRequest request)
        {
            return Task.CompletedTask;
        }
    }
}
