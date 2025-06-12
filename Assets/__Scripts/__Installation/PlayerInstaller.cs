using System;
using Course.Attribute;
using Course.Control;
using Course.Control.Player;
using Course.Core;
using Course.Effect;
using Course.ScriptableObject;
using Course.Utility;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.SocialPlatforms;
using Utility.DependencyInjection;

namespace Course.Installation
{
    public class PlayerInstaller : MonoBehaviour,IInstaller
    {
        [Header("PlayerShip Config")] 
        [SerializeField]
        private PlayerShipConfig config;
        
        [Header("Input")]
        [SerializeField] InputReader inputReader;
        
        [Header("Player Components")]
        [SerializeField] 
        PlayerShipBehaviour playerController;
        [SerializeField]
        HealthBehaviour healthBehaviour;
        [SerializeField]
        ScoreBehaviour scoreBehaviour;
        [SerializeField]
        JumpBehaviour jumpBehaviour;
        
        [Header("Turret Installer")]
        [SerializeField]
        TurretInstaller turretInstaller;
        
        [Header("Effect")]
        [SerializeField]
        OnDisappearEffect disappearEffect;
        [SerializeField]
        OnReAppearEffect reAppearEffect;
        
        // Cache
        private IMover _mover;
        private IDamageable _damageable;
        private IAttribute _score;
        private ITiltWithVelocity _tiltWithVelocity;
        private Rigidbody _rb;

        [Inject]
        private IAsteraX _manager;

        public void AwakeInitialize()
        {
            inputReader.Initialize();
            _damageable = new Damageable((float)config.JumpCount);
            healthBehaviour?.Initialize(_damageable);
            _score = new Score(config.Score);
            scoreBehaviour?.Initialize(_score);
            jumpBehaviour?.Initialize(healthBehaviour,_manager);
            turretInstaller?.Initialize(healthBehaviour,jumpBehaviour);
        }

        public void StartInitialize()
        {
            _mover = new Mover();
            _rb = GetComponent<Rigidbody>();
            _tiltWithVelocity = new TiltWithVelocity(
                transform,
                config.MaxTiltAngle,
                config.TiltSpeed,
                config.MoveSpeed
            );
            disappearEffect?.Initialize(jumpBehaviour);
            reAppearEffect?.Initialize(jumpBehaviour);
            new PlayerShipBehaviour.PlayerBuilder(playerController)
                .WithMover(_mover)
                .WithInputReader(inputReader)
                .WithMoveSpeed(config.MoveSpeed)
                .WithRigidbody(_rb)
                .WithTiltLogic(_tiltWithVelocity)
                .WithJumpBehaviour(jumpBehaviour)
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

