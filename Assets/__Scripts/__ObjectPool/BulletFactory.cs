using Course.Attribute.Bullet;
using Course.Core;

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
