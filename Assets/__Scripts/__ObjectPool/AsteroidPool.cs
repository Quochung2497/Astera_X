using Course.Attribute;
using Course.Utility;
using UnityEngine;

namespace Course.ObjectPool
{
    public class AsteroidPool : Pool<Asteroid>
    {
        private readonly Transform _anchor;
    
        public AsteroidPool(Asteroid prefab, Transform anchor)
            : base(prefab, collectionCheck: true)
        {
            _anchor = anchor;
        }
    
        protected override Asteroid OnCreate()
        {
            var a = Object.Instantiate(Prefab, _anchor);
            a.SetPool(PoolInstance);
            return a;
        }
        
        protected override void OnGet(Asteroid a)
        {
            a.transform.SetParent(_anchor, worldPositionStays: false);
            base.OnGet(a);
        }
    }
}

