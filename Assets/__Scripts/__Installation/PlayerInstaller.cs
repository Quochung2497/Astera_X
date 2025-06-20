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
        [Header("PlayerShip Config")] 
        [SerializeField]
        private PlayerShipConfig config;
        [SerializeField]
        private StatsProvider statsProvider = null;

        
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
        private ITiltWithVelocity _tiltWithVelocity;
        private Rigidbody _rb;
        private PlayerShipBehaviour.PlayerBuilder _builder;
        
        [Inject]
        private IAsteraX _manager;
        [Inject]
        private CustomizationManager _customizationManager;

        public void AwakeInitialize()
        {
            inputReader.Initialize();
            
            _damageable = new Damageable(config.JumpCount);
            statsProvider.RegisterStatInstance(Stats.Health, _damageable);
            healthBehaviour?.Initialize(_damageable);
            
            statsProvider?.RegisterStat(Stats.Score, config.Score);
            scoreBehaviour?.Initialize(statsProvider.Get(Stats.Score));

            statsProvider?.RegisterStat(Stats.Speed, config.MoveSpeed);
            jumpBehaviour?.Initialize(healthBehaviour,_manager);
            
            turretInstaller?.Initialize(healthBehaviour,jumpBehaviour);
            _customizationManager.OnApplyShipPart += HandleCustomizationChanged;
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
            ); // TODO: Refactor to configure
            disappearEffect?.Initialize(jumpBehaviour);
            reAppearEffect?.Initialize(jumpBehaviour);
            _builder = new PlayerShipBehaviour.PlayerBuilder(playerController)
                .WithMover(_mover)
                .WithInputReader(inputReader)
                .WithMoveSpeed(statsProvider.Get(Stats.Speed).Value)
                .WithRigidbody(_rb)
                .WithTiltLogic(_tiltWithVelocity)
                .WithJumpBehaviour(jumpBehaviour);
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
            inputReader.Shutdown();
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

