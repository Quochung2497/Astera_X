using System;
using Course.Attribute.Bullet;
using Course.Core;
using Course.Effect;
using Course.ScriptableObject;
using UnityEngine;
using UnityEngine.Serialization;

namespace Course.Installation
{
    public class BulletInstaller : MonoBehaviour, IInstaller
    {
        [SerializeField] private Bullet bullet = null;
        [SerializeField] private BulletConfig bulletConfig = null;
        // [SerializeField] private ExhaustTrailWrapBehaviour exhaustTrailWrapBehaviour = null;
        public void AwakeInitialize()
        {
            bullet.Initialize(bulletConfig.speed, bulletConfig.lifetime, bulletConfig.damage);
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