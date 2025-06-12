using Course.Attribute;
using Course.ScriptableObject;
using UnityEngine;

namespace Course.Core
{
    public interface IAsteraX
    {
        Transform asteroidParent { get; }
        AsteroidsConfig asteroidsSO { get; }

        Asteroid GetRandomAsteroidFromPool(int size);
        
        void AddAsteroid(Asteroid asteroid);
        void RemoveAsteroid(Asteroid asteroid);

        public Vector3 GetSafeSpawnPosition(
            float minDistAwayFromPlayer,
            float minDistAwayFromAsteroids,
            int maxAttempts
        );
    }
}
