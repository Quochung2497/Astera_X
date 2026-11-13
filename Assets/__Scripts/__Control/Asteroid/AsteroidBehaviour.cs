using System;
using Course.Attribute;
using Course.Core;
using Course.ScriptableObject;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Course.Control.Asteroid
{
    public class AsteroidBehaviour : IAsteroidBehaviour
    {
        #region Private Fields

        /// <summary>
        /// Reference to the AsteraX manager, used for accessing asteroid-related settings and operations.
        /// </summary>
        private readonly IAsteraX _manager;

        /// <summary>
        /// Callback action for setting up an asteroid's attributes.
        /// </summary>
        private readonly Action<IAsteroid,int> _setup;

        private readonly IScreenBounds _screenBounds;
        private readonly AsteroidsConfig _config;

        /// <summary>
        /// Reserved callback for asteroid collision handling.
        /// </summary>
        readonly Action<IAsteroid, Collision> _onHitCallback;

        #endregion

        #region Public Methods

          /// <summary>
        /// Initializes a new instance of the <see cref="AsteroidBehaviour"/> class.
        /// Sets up the manager, setup callback, out-of-bounds checker and reserved collision callback.
        /// </summary>
        /// <param name="manager">The AsteraX manager instance.</param>
        /// <param name="setupCallback">Callback for setting up asteroid attributes.</param>
        /// <param name="screenBounds">The scene bounds used to choose an initial direction.</param>
        /// <param name="config">The asteroid velocity settings.</param>
        /// <param name="onHitCallback">Reserved callback for asteroid collision handling.</param>
        public AsteroidBehaviour(IAsteraX manager,
            Action<IAsteroid,int> setupCallback,
            IScreenBounds screenBounds,
            AsteroidsConfig config,
            Action<IAsteroid, Collision> onHitCallback
            )
        {
            _manager = manager;
            _setup = setupCallback;
            _screenBounds = screenBounds;
            _config = config;
            _onHitCallback = onHitCallback;
        }
        
        public void SpawnDescendent(    
            int childSize,
            int childCount,
            int childHealth,
            int childPoints,
            int childDamage,

            int grandchildSize,
            int grandchildCount,
            int grandchildHealth,
            int grandchildPoints,
            int grandchildDamage,

            Transform parentTransform,
            string parentName)
        {
            if (childCount <= 0 || childSize <= 0) return;

            for (int i = 0; i < childCount; i++)
            {
                var frag = _manager.GetRandomAsteroidFromPool(childSize);
                frag.gameObject.name = $"{parentName}_{i:00}";
                frag.transform.SetParent(parentTransform, worldPositionStays: false);
                frag.SetPointValue(childPoints); 
                frag.SetDamageValue(childDamage);
                _setup(frag, childHealth);  // calls Setup()

                frag.transform.localPosition = Random.onUnitSphere * 0.5f;
                frag.transform.localRotation = Random.rotation;

                // Now pass next generation’s sizes & counts:
                SpawnDescendent(
                    grandchildSize,
                    grandchildCount,
                    grandchildHealth,
                    grandchildPoints,
                    grandchildDamage,
                      0,             
                      0,
                      0,
                      0,
                      0,
                    frag.transform,
                    frag.gameObject.name
                    );
            }
        }

        /// <summary>
        /// Calculates the initial velocity of an asteroid based on its position and size.
        /// Determines the linear and angular velocity using random values and asteroid settings.
        /// </summary>
        /// <param name="currentPosition">The current position of the asteroid.</param>
        /// <param name="size">The size of the asteroid.</param>
        /// <returns>A <see cref="VelocityInfo"/> struct containing the linear and angular velocity.</returns>
        public VelocityInfo CalculateInitialVelocity(Vector3 currentPosition, int size)
        {
            return AsteroidVelocityCalculator.Calculate(
                currentPosition, size, _screenBounds.OOB(currentPosition),
                _config.minVel, _config.maxVel, _config.maxAngularVel);
        }

        /// <summary>
        /// Releases the direct children only after their parent's health reaches zero.
        /// Grandchildren stay attached until their own parent dies.
        /// </summary>
        public void OnDeath(IAsteroid self)
        {
            if (self.size > 1)
            {
                var children = self.gameObject.GetComponentsInChildren<IAsteroid>();
                foreach (var child in children)
                {
                    if (child == self || child.transform.parent != self.transform)
                        continue;

                    child.transform.SetParent(_manager.asteroidParent, worldPositionStays: true);
                    child.InitAsteroid();
                }
            }
        }

        #endregion
      
    }
    
    /// <summary>
    /// Represents the velocity information of an asteroid, including linear and angular velocity.
    /// </summary>
    public struct VelocityInfo
    {
        /// <summary>
        /// The linear velocity of the asteroid.
        /// </summary>
        public Vector3 Linear;
        
        /// <summary>
        /// The angular velocity of the asteroid.
        /// </summary>
        public Vector3 Angular;
    }
}
