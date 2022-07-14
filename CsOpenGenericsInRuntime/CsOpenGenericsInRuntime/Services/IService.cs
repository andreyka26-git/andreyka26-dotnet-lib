namespace CsOpenGenericsInRuntime.Services
{
    public interface IService<in T>
        where T : class, IServiceRequest
    {
        public Task HandleAsync(T request);
    }
}
