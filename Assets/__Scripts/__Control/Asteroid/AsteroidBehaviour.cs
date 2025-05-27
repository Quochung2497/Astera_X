using System;
using Course.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Course.Control.Asteroid
{
    public class AsteroidBehaviour : IAsteroidBehaviour
    {
        private readonly IAsteraX _manager;
        readonly Action<Attribute.Asteroid> _setup;
        readonly Func<Vector3, bool> _isOutOfBounds;
        readonly Action<Attribute.Asteroid>             _releaseCallback;
        readonly Action<Attribute.Asteroid, Collision>  _onHitCallback;
        
        public AsteroidBehaviour(IAsteraX manager,
            Action<Attribute.Asteroid> setupCallback,
            Func<Vector3,bool> isOutOfBoundsChecker,
            Action<Attribute.Asteroid, Collision>  onHitCallback,
            Action<Attribute.Asteroid>             releaseCallback
            )
        {
            _manager = manager;
            _setup = setupCallback;
            _isOutOfBounds   = isOutOfBoundsChecker;
            _releaseCallback = releaseCallback;
            _onHitCallback   = onHitCallback;
        }
        
        public void SpawnChildren(int sizeLevel,Transform parentTransform, string parentName)
        {
            if (sizeLevel <= 1)
                return;

            var so = _manager.asteroidsSO;
            for (int i = 0; i < so.numSmallerAsteroidsToSpawn; i++)
            {
                var frag = _manager.GetRandomAsteroidFromPool();

                frag.name = $"{parentName}_{i:00}";
                frag.transform.SetParent(parentTransform, worldPositionStays: false);
                frag.SetSize(sizeLevel - 1);
                _setup(frag);

                frag.transform.localPosition = Random.onUnitSphere / 2f;
                frag.transform.localRotation = Random.rotation;

                SpawnChildren(sizeLevel - 1, frag.transform, frag.name);
            }
        }

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

            _releaseCallback(self);
        }
    }
    
    public struct VelocityInfo
    {
        public Vector3 Linear;
        public Vector3 Angular;
    }
}
