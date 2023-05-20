namespace Concurrency.Lightweight
{
    /// <summary>
    /// Note this one cannot be used with async/await
    /// </summary>
    public class WriteLockService
    {
        private TimeSpan _lockWaitTimeout = TimeSpan.FromMinutes(1);
        private readonly ReaderWriterLockSlim _readWriteLock = new ReaderWriterLockSlim();

        public void Read(Action func)
        {
            if (!_readWriteLock.TryEnterReadLock(_lockWaitTimeout))
            {
                throw new Exception($"{nameof(Read)} cannot enter read lock");
            }

            try
            {
                func();
            }
            finally
            {
                _readWriteLock.ExitReadLock();
            }
        }

        public T Read<T>(Func<T> func)
        {
            if (!_readWriteLock.TryEnterReadLock(_lockWaitTimeout))
            {
                throw new Exception($"{nameof(Read)} cannot enter read lock");
            }

            try
            {
                return func();
            }
            finally
            {
                _readWriteLock.ExitReadLock();
            }
        }


        public void Write(Action func)
        {
            if (!_readWriteLock.TryEnterWriteLock(_lockWaitTimeout))
            {
                throw new Exception($"{nameof(Write)} cannot enter write lock");
            }

            try
            {
                func();
            }
            finally
            {
                _readWriteLock.ExitWriteLock();
            }
        }

        public T Write<T>(Func<T> func)
        {
            if (!_readWriteLock.TryEnterWriteLock(_lockWaitTimeout))
            {
                throw new Exception($"{nameof(Write)} cannot enter write lock");
            }

            try
            {
                return func();
            }
            finally
            {
                _readWriteLock.ExitWriteLock();
            }
        }
    }
}
