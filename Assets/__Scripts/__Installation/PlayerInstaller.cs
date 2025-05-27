using System;
using Course.Control;
using Course.Control.Player;
using Course.Core;
using Course.Utility;
using UnityEngine;

namespace Course.Installation
{
    public class PlayerInstaller : MonoBehaviour,IInstaller
    {
        [Header("Movement")]
        [SerializeField] float moveSpeed = 10f;
        
        [Header("Input")]
        [SerializeField] InputReader inputReader;
        
        [Header("Player")]
        [SerializeField] 
        PlayerMovementBehaviour playerController;
        
        [Header("Tilt Settings")]
        [Tooltip("Max bank angle (degrees) when strafing at full speed")]
        [SerializeField] float maxTiltAngle = 30f;
        [Tooltip("How quickly the ship tilts toward the target angle")]
        [SerializeField] float tiltSpeed = 5f;
        
        // Cache
        private IMover _mover;
        private ITiltWithVelocity _tiltWithVelocity;
        private Rigidbody _rb;

        public void AwakeInitialize()
        {
            inputReader.Initialize();
        }

        public void StartInitialize()
        {
            _mover = new Mover();
            _rb = GetComponent<Rigidbody>();
            _tiltWithVelocity = new TiltWithVelocity(
                transform,
                maxTiltAngle,
                tiltSpeed,
                moveSpeed
            );
            new PlayerMovementBehaviour.PlayerBuilder(playerController)
                .WithMover(_mover)
                .WithInputReader(inputReader)
                .WithMoveSpeed(moveSpeed)
                .WithRigidbody(_rb)
                .WithTiltLogic(_tiltWithVelocity)
                .Build();
        }
        
        private void Awake()
        {
            AwakeInitialize();
        }
        
        private void Start()
        {
            StartInitialize();
        }

        private void OnDestroy()
        {
            inputReader.Shutdown();
        }
    }
}

