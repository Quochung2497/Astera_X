using Course.Attribute;
using Course.Utility;
using UnityEngine;

namespace Course.ObjectPool
{
    public class AsteroidPool : Pool<Asteroid>
    {
        /// <summary>
        /// The anchor transform where the asteroids will be instantiated and managed.
        /// </summary>
        private readonly Transform _anchor;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsteroidPool"/> class.
        /// </summary>
        /// <param name="prefab">The asteroid prefab to be pooled.</param>
        /// <param name="anchor">The transform that serves as the parent for instantiated asteroids.</param>
        public AsteroidPool(Asteroid prefab, Transform anchor)
            : base(prefab, collectionCheck: true)
        {
            _anchor = anchor;
        }

        /// <summary>
        /// Creates a new asteroid instance and sets its pool reference.
        /// </summary>
        /// <returns>The newly created asteroid instance.</returns>
        protected override Asteroid OnCreate()
        {
            var a = Object.Instantiate(Prefab, _anchor);
            a.SetPool(PoolInstance);
            return a;
        }

        /// <summary>
        /// Handles the retrieval of an asteroid from the pool.
        /// Sets the parent transform of the asteroid to the anchor.
        /// </summary>
        /// <param name="a">The asteroid instance being retrieved.</param>
        protected override void OnGet(Asteroid a)
        {
            a.transform.SetParent(_anchor, worldPositionStays: false);
            base.OnGet(a);
        }
    }
}

