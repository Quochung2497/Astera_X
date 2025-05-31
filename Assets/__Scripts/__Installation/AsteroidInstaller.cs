using System;
using Course.Attribute;
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
        [SerializeField] private HealthBehaviour healthBehaviour;
        [SerializeField] private AsteroidOnDeathEffect asteroidOnDeathEffect;
        
        public void AwakeInitialize()
        {
            asteroid.Initialize(offScreenWrapper, healthBehaviour, asteroidOnDeathEffect);
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