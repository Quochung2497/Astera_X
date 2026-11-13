using Course.Attribute;
using Course.Control;
using Course.Control.Player;
using Course.Core;
using Course.Effect;
using Course.ScriptableObject;
using Course.Utility;
using UnityEngine;
using Utility.DependencyInjection;

namespace Course.Installation
{
    public class PlayerInstaller : MonoBehaviour,IInstaller
    {
        [Header("Player Stats")]
        [SerializeField]
        private StatsProvider statsProvider = null;

        
        
        [Header("Player Components")]
        [SerializeField] 
        PlayerShipBehaviour playerController;
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
        private ITiltWithVelocity _tiltWithVelocity;
        private Rigidbody _rb;
        private PlayerShipBehaviour.PlayerBuilder _builder;
        private PlayerActionState _actions;
        
        [Inject] private PlayerShipConfig _config;
        [Inject] private IDamageable _damageable;
        [Inject] private IHealthBehaviour _health;
        [Inject] private IInputReader _inputReader;
        [Inject] private IGameStateReader _gameState;

        [Inject]
        private IAsteraX _manager;
        [Inject]
        private ICustomization _customizationManager;

        public void AwakeInitialize()
        {
            statsProvider.RegisterStatInstance(Stats.Health, _damageable);
            
            statsProvider?.RegisterStat(Stats.Score, _config.Score);
            scoreBehaviour?.Initialize(statsProvider.Get(Stats.Score));

            statsProvider?.RegisterStat(Stats.Speed, _config.MoveSpeed);
            _actions = new PlayerActionState(_health, _gameState);
            jumpBehaviour?.Initialize(_health, _manager, _actions);
            
            turretInstaller?.Initialize(_actions);
            _customizationManager.OnApplyShipPart += HandleCustomizationChanged;
        }

        public void StartInitialize()
        {
            _mover = new Mover();
            _rb = GetComponent<Rigidbody>();
            _tiltWithVelocity = new TiltWithVelocity(
                transform,
                _config.MaxTiltAngle,
                _config.TiltSpeed,
                _config.MoveSpeed
            ); // TODO: Refactor to configure
            disappearEffect?.Initialize(jumpBehaviour);
            reAppearEffect?.Initialize(jumpBehaviour);
            _builder = new PlayerShipBehaviour.PlayerBuilder(playerController)
                .WithMover(_mover)
                .WithInputReader(_inputReader)
                .WithMoveSpeed(statsProvider.Get(Stats.Speed).Value)
                .WithRigidbody(_rb)
                .WithTiltLogic(_tiltWithVelocity)
                .WithJumpBehaviour(jumpBehaviour)
                .WithActionState(_actions);
            _builder.Build();
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
            _customizationManager.OnApplyShipPart -= HandleCustomizationChanged;
        }
        
        private void HandleCustomizationChanged(PartType part, Transform mount, ShipPartSO.StatModifier[] mods)
        {
            if (part != PartType.Body) return;
            foreach (var mod in mods)
                statsProvider.SetMaxValue(mod.stats, mod.percentBonus);
            RebuildPlayer();
        }

        private void RebuildPlayer()
        {
            float moveSpeed = statsProvider.Get(Stats.Speed).MaxValue;
            float maxHealth = statsProvider.Get(Stats.Health).MaxValue;

            _builder.WithMoveSpeed(moveSpeed).Build();
            
            _damageable.SetMaxValue(maxHealth);
        }
    }
}

