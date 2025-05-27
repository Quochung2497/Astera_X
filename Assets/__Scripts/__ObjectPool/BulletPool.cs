using Course.Attribute.Bullet;
using Course.Utility;
using UnityEngine;

namespace Course.ObjectPool
{
    public class BulletPool: Pool<Bullet>
    {
        private Transform _parent;
        public BulletPool(Bullet prefab, Transform parent, int maxSize = 16, int defaultCapacity = 16)
            : base(prefab, true, maxSize, defaultCapacity)
        {
            _parent = parent;
        }
        protected override Bullet OnCreate()
        {
            var bulletPool = GameObject.Instantiate(Prefab, _parent);
            bulletPool.SetPool(PoolInstance);
            return bulletPool;
        }
        protected override void OnRelease(Bullet b)
        {
            b.transform.SetParent(_parent, true);
            base.OnRelease(b);
        }
    }
}
