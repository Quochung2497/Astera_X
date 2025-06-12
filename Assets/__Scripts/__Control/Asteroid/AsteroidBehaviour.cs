using System;
using Course.Core;
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
        private readonly Action<Course.Attribute.Asteroid,int> _setup;

        /// <summary>
        /// Function to check if a given position is out of bounds.
        /// </summary>
        readonly Func<Vector3, bool> _isOutOfBounds;

        /// <summary>
        /// Callback action for handling asteroid collision events.
        /// </summary>
        readonly Action<Attribute.Asteroid, Collision> _onHitCallback;

        #endregion

        #region Public Methods

          /// <summary>
        /// Initializes a new instance of the <see cref="AsteroidBehaviour"/> class.
        /// Sets up the manager, setup callback, out-of-bounds checker, collision callback, and release callback.
        /// </summary>
        /// <param name="manager">The AsteraX manager instance.</param>
        /// <param name="setupCallback">Callback for setting up asteroid attributes.</param>
        /// <param name="isOutOfBoundsChecker">Function to check if a position is out of bounds.</param>
        /// <param name="onHitCallback">Callback for handling asteroid collision events.</param>
        public AsteroidBehaviour(IAsteraX manager,
            Action<Course.Attribute.Asteroid,int> setupCallback,
            Func<Vector3,bool> isOutOfBoundsChecker,
            Action<Attribute.Asteroid, Collision>  onHitCallback
            )
        {
            _manager = manager;
            _setup = setupCallback;
            _isOutOfBounds   = isOutOfBoundsChecker;
            _onHitCallback   = onHitCallback;
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
                frag.name = $"{parentName}_{i:00}";
                frag.transform.SetParent(parentTransform, worldPositionStays: false);
                frag.SetPointValue(childPoints); 
                frag.SetDamageValue(childDamage);
                _setup(frag, childHealth);  // calls Setup()
                _manager.AddAsteroid(frag);  // ensure manager knows about this new child

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
                    frag.name
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
            Vector3 vel;
            if (_isOutOfBounds(currentPosition))
            {
                vel = ((Vector3)Random.insideUnitCircle * 4f) - currentPosition;
                vel.Normalize();
            }
            else
            {
                do
                {
                    vel = Random.insideUnitCircle;
                    vel.Normalize();
                }
                while (Mathf.Approximately(vel.magnitude, 0f));
            }

            var so = _manager.asteroidsSO;
            var speed = Random.Range(so.minVel, so.maxVel) / (float)size;
            vel *= speed;

            var ang = Random.insideUnitSphere * so.maxAngularVel;

            return new VelocityInfo {
                Linear  = vel,
                Angular = ang
            };
        }

        /// <summary>
        /// Handles the logic when an asteroid is hit.
        /// If the asteroid has children, reinitializes them and detaches them from the parent.
        /// Delegates the release logic to the provided release callback.
        /// </summary>
        /// <param name="self">The asteroid that was hit.</param>
        /// <param name="collision">The collision data.</param>
        public void OnHit(Attribute.Asteroid self, Collision collision)
        {
            if (self.size > 1)
            {
                var children = self.GetComponentsInChildren<Attribute.Asteroid>();
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
