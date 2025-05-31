using UnityEngine;
using UnityEngine.Pool;

namespace Course.Core
{
    public abstract class BaseFactory<T> : IFactory<T> where T : class
    {
        /// <summary>
        /// The object pool used to manage instances of type <typeparamref name="T"/>.
        /// </summary>
        protected readonly IObjectPool<T> _pool;

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseFactory{T}"/> class.
        /// </summary>
        /// <param name="pool">The object pool responsible for managing instances of type <typeparamref name="T"/>.</param>
        protected BaseFactory(IObjectPool<T> pool)
        {
            _pool = pool;
        }

        /// <summary>
        /// Retrieves an instance of type <typeparamref name="T"/> from the object pool.
        /// </summary>
        /// <returns>An instance of type <typeparamref name="T"/>.</returns>
        public virtual T Get()
        {
            return _pool.Get();
        }
    }
}

