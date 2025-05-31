using Course.Attribute.Bullet;
using Course.Utility;
using UnityEngine;

namespace Course.ObjectPool
{
    public class BulletPool: Pool<Bullet>
    {
        /// <summary>
        /// The parent transform where bullets will be instantiated and managed.
        /// </summary>
        private Transform _parent;

        /// <summary>
        /// Initializes a new instance of the <see cref="BulletPool"/> class.
        /// </summary>
        /// <param name="prefab">The bullet prefab to be pooled.</param>
        /// <param name="parent">The transform that serves as the parent for instantiated bullets.</param>
        /// <param name="maxSize">The maximum number of bullets allowed in the pool.</param>
        /// <param name="defaultCapacity">The initial capacity of the pool.</param>
        public BulletPool(Bullet prefab, Transform parent, int maxSize = 16, int defaultCapacity = 16)
            : base(prefab, true, maxSize, defaultCapacity)
        {
            _parent = parent;
        }

        /// <summary>
        /// Creates a new bullet instance and sets its pool reference.
        /// </summary>
        /// <returns>The newly created bullet instance.</returns>
        protected override Bullet OnCreate()
        {
            var bulletPool = GameObject.Instantiate(Prefab, _parent);
            bulletPool.SetPool(PoolInstance);
            return bulletPool;
        }

        /// <summary>
        /// Handles the release of a bullet back to the pool.
        /// Sets the parent transform of the bullet to the specified parent.
        /// </summary>
        /// <param name="b">The bullet instance being released.</param>
        protected override void OnRelease(Bullet b)
        {
            b.transform.SetParent(_parent, true);
            base.OnRelease(b);
        }
    }
}
