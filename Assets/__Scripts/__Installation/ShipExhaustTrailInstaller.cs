using System;
using Course.Core;
using Course.Effect;
using UnityEngine;

namespace Course.Installation
{
    public class ShipExhaustTrailInstaller : MonoBehaviour,IInstaller
    {
        [Header("Exhaust Trail Effect")]
        [SerializeField] private ExhaustTrailWrapBehaviour exhaustTrailWrapBehaviour;

        [SerializeField] private EffectFollowGO effectFollower;

        private const string PlayerShipTag = "Player";
        
        public void AwakeInitialize()
        {
            var playerShip = GameObject.FindGameObjectWithTag(PlayerShipTag);
            exhaustTrailWrapBehaviour?.Initialize(playerShip.transform,playerShip.TryGetComponent<IOffScreenWrapper>(out var offScreenWrapper) ? offScreenWrapper : null);
            effectFollower?.Initialize(playerShip.transform);
        }

        public void StartInitialize()
        {
            // No start initialization needed for exhaust trail
        }

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

