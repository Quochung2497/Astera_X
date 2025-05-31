using UnityEngine;
using UnityEngine.Pool;

namespace Course.Utility
{
    /// <summary>
    /// Abstract base class for managing object pooling in Unity.
    /// Provides functionality for creating, retrieving, releasing, and destroying pooled objects.
    /// </summary>
    /// <typeparam name="T">The type of objects managed by the pool. Must inherit from MonoBehaviour and implement IPoolObject&lt;T&gt;.</typeparam>
    public abstract class Pool<T> : IPool<T>
      where T : MonoBehaviour, IPoolObject<T>
    {
      /// <summary>
      /// The prefab used to create new instances of the pooled object.
      /// </summary>
      protected readonly T Prefab;

      /// <summary>
      /// The instance of the object pool managing the pooled objects.
      /// </summary>
      protected readonly IObjectPool<T> PoolInstance;

      /// <summary>
      /// Initializes a new instance of the <see cref="Pool{T}"/> class.
      /// </summary>
      /// <param name="prefab">The prefab used to create new instances of the pooled object.</param>
      /// <param name="collectionCheck">Whether to check for duplicate objects in the pool.</param>
      /// <param name="maxSize">The maximum number of objects allowed in the pool.</param>
      /// <param name="defaultCapacity">The initial capacity of the pool.</param>
      public Pool(T prefab, bool collectionCheck = true, int maxSize = 10, int defaultCapacity = 10)
      {
        Prefab = prefab;

        PoolInstance = new ObjectPool<T>(
          OnCreate,
          OnGet,
          OnRelease,
          OnDestroyObject,
          maxSize: maxSize,
          defaultCapacity: defaultCapacity
        );
      }

      /// <summary>
      /// Retrieves the instance of the object pool managing the pooled objects.
      /// </summary>
      /// <returns>The object pool instance.</returns>
      public IObjectPool<T> GetPool()
      {
        return PoolInstance;
      }

      /// <summary>
      /// Creates a new instance of the pooled object.
      /// Sets the pool reference on the created object.
      /// </summary>
      /// <returns>The newly created instance of the pooled object.</returns>
      protected virtual T OnCreate()
      {
        var objectInstance = GameObject.Instantiate(Prefab);
        objectInstance.SetPool(PoolInstance);
        return objectInstance;
      }

      /// <summary>
      /// Called when an object is retrieved from the pool.
      /// Activates the object in the scene.
      /// </summary>
      /// <param name="poolObject">The object being retrieved from the pool.</param>
      protected virtual void OnGet(T poolObject)
      {
        poolObject.gameObject.SetActive(true);
      }

      /// <summary>
      /// Called when an object is released back to the pool.
      /// Deactivates the object in the scene.
      /// </summary>
      /// <param name="poolObject">The object being released back to the pool.</param>
      protected virtual void OnRelease(T poolObject)
      {
        poolObject.gameObject.SetActive(false);
      }

      /// <summary>
      /// Called when an object is destroyed by the pool.
      /// Destroys the object's GameObject in the scene.
      /// </summary>
      /// <param name="poolObject">The object being destroyed.</param>
      protected virtual void OnDestroyObject(T poolObject)
      {
        GameObject.Destroy(poolObject.gameObject);
      }
    }
}
