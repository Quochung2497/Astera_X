using UnityEngine;
using UnityEngine.Pool;

namespace Course.Core
{
    public abstract class BaseFactory<T> : IFactory<T> where T : class
    {
        protected readonly IObjectPool<T> _pool;

        protected BaseFactory(IObjectPool<T> pool)
        {
            _pool = pool;
        }

        public virtual T Get()
        {
            return _pool.Get();
        }
    }
}

