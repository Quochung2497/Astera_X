using Course.Attribute;
using Course.Core;
using Course.Installation;
using Course.ScriptableObject;
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
        private readonly IScreenBounds _screenBounds;
        private readonly AsteroidsConfig _config;

        /// <summary>
        /// Initializes a new instance of the <see cref="AsteroidPool"/> class.
        /// </summary>
        /// <param name="prefab">The asteroid prefab to be pooled.</param>
        /// <param name="anchor">The transform that serves as the parent for instantiated asteroids.</param>
        /// <param name="screenBounds">The scene bounds used to initialize asteroid movement.</param>
        /// <param name="config">The asteroid settings used to calculate initial velocity.</param>
        public AsteroidPool(Asteroid prefab, Transform anchor, IScreenBounds screenBounds, AsteroidsConfig config)
            : base(prefab, collectionCheck: true)
        {
            _anchor = anchor;
            _screenBounds = screenBounds;
            _config = config;
        }

        /// <summary>
        /// Creates a new asteroid instance and sets its pool reference.
        /// </summary>
        /// <returns>The newly created asteroid instance.</returns>
        protected override Asteroid OnCreate()
        {
            var a = Object.Instantiate(Prefab, _anchor);
            a.GetComponent<AsteroidInstaller>().Initialize(_screenBounds, _config);
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

