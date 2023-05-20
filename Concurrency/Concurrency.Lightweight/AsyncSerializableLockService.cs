namespace Concurrency.Lightweight
{
    public class AsyncSerializableLockService
    {
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly TimeSpan _timeout;

        public AsyncSerializableLockService(TimeSpan timeout)
        {
            _timeout = timeout;
        }

        public async Task ExecuteSeriallyAsync(Func<Task> func, CancellationToken cancellationToken = default)
        {
            try
            {
                await _semaphore.WaitAsync(_timeout, cancellationToken);

                await func();
            }
            finally
            {
                _semaphore.Release();
            }
        }

        public async Task<T> ExecuteSeriallyAsync<T>(Func<Task<T>> func, CancellationToken cancellationToken = default)
        {
            try
            {
                await _semaphore.WaitAsync(_timeout, cancellationToken);

                return await func();
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
