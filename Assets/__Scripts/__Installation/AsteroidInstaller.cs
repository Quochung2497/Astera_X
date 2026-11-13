using Course.Attribute;
using Course.Control.Asteroid;
using Course.Core;
using Course.Effect;
using Course.ScriptableObject;
using UnityEngine;

namespace Course.Installation
{
    public class AsteroidInstaller : MonoBehaviour
    {
        [SerializeField] private Asteroid asteroid;
        [SerializeField] private OffScreenWrapper offScreenWrapper;
        [SerializeField] private AsteroidOnDeathEffect asteroidOnDeathEffect;

        internal void Initialize(IScreenBounds screenBounds, AsteroidsConfig config)
        {
            var health = new AsteroidHealth(hp => new Damageable(hp));
            asteroid.Initialize(offScreenWrapper, health, screenBounds, config);
            asteroidOnDeathEffect?.Initialize(asteroid);
        }
    }
}