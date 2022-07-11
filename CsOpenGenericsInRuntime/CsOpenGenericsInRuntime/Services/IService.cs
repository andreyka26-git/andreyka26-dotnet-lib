namespace CsOpenGenericsInRuntime.Services
{
    public interface IService<T>
        where T : class, IServiceRequest
    {
        public Task HandleAsync(T request);
    }
}
