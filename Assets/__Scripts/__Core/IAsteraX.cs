using Course.Attribute;
using UnityEngine;

namespace Course.Core
{
    public interface IAsteraX
    {
        Transform asteroidParent { get; }
        AsteroidsScriptableObject asteroidsSO { get; }

        Asteroid GetRandomAsteroidFromPool();
        
        void AddAsteroid(Asteroid asteroid);
        void RemoveAsteroid(Asteroid asteroid);
    }
}
