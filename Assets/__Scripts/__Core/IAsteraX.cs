using Course.Attribute;
using Course.ScriptableObject;
using UnityEngine;

namespace Course.Core
{
    public interface IAsteraX
    {
        Transform asteroidParent { get; }
        AsteroidsConfig asteroidsSO { get; }

        IAsteroid GetRandomAsteroidFromPool(int size);
        
        void AddAsteroid(IAsteroid asteroid);
        void RemoveAsteroid(IAsteroid asteroid);

        public Vector3 GetSafeSpawnPosition(
            float minDistAwayFromPlayer,
            float minDistAwayFromAsteroids,
            int maxAttempts
        );
    }
}
