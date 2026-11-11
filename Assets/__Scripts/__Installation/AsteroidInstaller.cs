using Course.Attribute;
using Course.Control.Asteroid;
using Course.Core;
using Course.Effect;
using UnityEngine;

namespace Course.Installation
{
    [DefaultExecutionOrder(-100)]
    public class AsteroidInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] private Asteroid asteroid;
        [SerializeField] private OffScreenWrapper offScreenWrapper;
        [SerializeField] private AsteroidOnDeathEffect asteroidOnDeathEffect;
        
        public void AwakeInitialize()
        {
            var health = new AsteroidHealth(hp => new Damageable(hp));
            asteroid.Initialize(offScreenWrapper, health);
            asteroidOnDeathEffect?.Initialize(asteroid);
        }

        public void StartInitialize() { }

        private void Awake()
        {
            AwakeInitialize();
        }
        
        private void Start()
        {
            StartInitialize();
        }
    }
}