using Course.Attribute;
using Course.Core;

namespace Course.ObjectPool
{
    public class AsteroidFactory : BaseFactory<Asteroid>
    {
        public AsteroidFactory(AsteroidPool pool) : base(pool.GetPool())
        {
        }
    }
}

