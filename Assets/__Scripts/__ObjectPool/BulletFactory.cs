using Course.Attribute.Bullet;
using Course.Core;
using UnityEngine;

namespace Course.ObjectPool
{
    public class BulletFactory: BaseFactory<Bullet>
    {
        public BulletFactory(BulletPool bulletPool)
            : base(bulletPool.GetPool())
        {
        }
    }
}
