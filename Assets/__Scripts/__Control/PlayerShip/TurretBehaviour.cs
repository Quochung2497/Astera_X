using System;
using Course.Attribute;
using Course.Core;
using Course.Utility;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Course.Control.Turret
{
    public class TurretBehaviour : MonoBehaviour
    {
        #region Serialized Fields

        /// <summary>
        /// Indicates whether to draw a debug line for aiming visualization.
        /// </summary>
        [SerializeField] private bool debugDrawLine = false;

        #endregion

        #region Private Fields

        /// <summary>
        /// Reference to the input reader for handling player input events.
        /// </summary>
        private InputReader _inputReader;

        /// <summary>
        /// Reference to the rotator component responsible for aiming the turret.
        /// </summary>
        private IRotator _rotator;

        /// <summary>
        /// Reference to the shooter component responsible for firing projectiles.
        /// </summary>
        private IShooter _shooter;

        /// <summary>
        /// Reference to the jump behavior component for handling jump-related actions.
        /// </summary>
        private IHealthBehaviour _healthBehaviour;

        /// <summary>
        /// Transform representing the turret object.
        /// </summary>
        private Transform _turret;

        /// <summary>
        /// Transform representing the spawn point for projectiles.
        /// </summary>
        private Transform _spawn;

        /// <summary>
        /// Reference to the main camera used for aiming calculations.
        /// </summary>
        private Camera _cam;

        /// <summary>
        /// Stores the screen position for debug drawing purposes.
        /// </summary>
        private Vector2 _screenPos;

        /// <summary>
        /// Indicates whether the turret behavior has been initialized.
        /// </summary>
        private bool _isInitialized = false;
        #endregion

        #region Private Methods
        
        /// <summary>
        /// Initializes the turret behavior with the specified components and settings.
        /// </summary>
        /// <param name="inputReader">The input reader for handling player input.</param>
        /// <param name="rotator">The rotator component for aiming the turret.</param>
        /// <param name="shooter">The shooter component for firing projectiles.</param>
        /// <param name="turret">The transform of the turret object.</param>
        /// <param name="spawn">The transform of the spawn point for projectiles.</param>
        /// <param name="healthBehaviour">The jump behavior component.</param>
        private void Initialize(InputReader inputReader,
            IRotator rotator,
            IShooter shooter,
            Transform turret,
            Transform spawn,
            IHealthBehaviour healthBehaviour)
        {
            _inputReader = inputReader;
            _rotator = rotator;
            _shooter = shooter;
            _turret = turret;
            _spawn = spawn;
            _cam = Camera.main;
            _healthBehaviour = healthBehaviour;
            _inputReader.OnAimEvent += OnAim;
            _inputReader.OnFireEvent += OnFire;
            _isInitialized = true;
        }
        
        /// <summary>
        /// Handles the aiming logic based on the provided screen position.
        /// </summary>
        /// <param name="screenPos">The screen position for aiming.</param>
        private void OnAim(Vector2 screenPos)
        {
            if (_healthBehaviour.IsDead)
                return;
            _rotator.Aim(_turret, screenPos, _cam);
            if (debugDrawLine)
            {
                float zDist = _turret.position.z - _cam.transform.position.z;
                _screenPos = _cam.ScreenToWorldPoint(
                    new Vector3(screenPos.x, screenPos.y, zDist));
            }
        }
        
        /// <summary>
        /// Handles the firing logic, including aiming and shooting projectiles.
        /// </summary>
        private void OnFire()
        {
            if (_healthBehaviour.IsDead)
                return;
            Vector2 screenPos;
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            {
                // Mobile tap
                screenPos = Touchscreen.current.primaryTouch.position.ReadValue();
            }
            else
            {
                // Mouse click or gamepad fallback
                screenPos = Mouse.current.position.ReadValue();
            }
        
            _rotator.Aim(_turret, screenPos, _cam);
            if (debugDrawLine)
            {
                float zDist = _turret.position.z - _cam.transform.position.z;
                _screenPos = _cam.ScreenToWorldPoint(
                    new Vector3(screenPos.x, screenPos.y, zDist));
            }
        
            _shooter.Fire(_turret, _spawn);
        }

        #endregion

        #region Unity API

        /// <summary>
        /// Unity's OnEnable method, called when the object becomes enabled.
        /// Re-subscribes to input events if initialized.
        /// </summary>
        private void OnEnable()
        {
            if (!_isInitialized)
                return;
            _inputReader.OnAimEvent += OnAim;
            _inputReader.OnFireEvent += OnFire;
        }

        /// <summary>
        /// Unity's OnDisable method, called when the object becomes disabled.
        /// Unsubscribes from input events if initialized.
        /// </summary>
        private void OnDisable()
        {
            if (!_isInitialized)
                return;
            _inputReader.OnAimEvent -= OnAim;
            _inputReader.OnFireEvent -= OnFire;
        }

        /// <summary>
        /// Unity's OnDrawGizmos method, called to draw gizmos in the editor.
        /// Draws a debug line and sphere for aiming visualization if enabled.
        /// </summary>
        private void OnDrawGizmos()
        {
            if (debugDrawLine && Application.isPlaying)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(_turret.position, _screenPos);
                Gizmos.DrawSphere(_screenPos, 0.2f);
            }
        }


        #endregion

        #region Builder

        /// <summary>
        /// Builder class for constructing and initializing a TurretBehaviour instance.
        /// </summary>
        public class TurretBuilder
        {
            /// <summary>
            /// The TurretBehaviour instance to build.
            /// </summary>
            readonly TurretBehaviour _behaviour;
        
            /// <summary>
            /// Reference to the input reader for handling player input events.
            /// </summary>
            InputReader _inputReader;
        
            /// <summary>
            /// Reference to the rotator component responsible for aiming the turret.
            /// </summary>
            IRotator _rotator;
        
            /// <summary>
            /// Reference to the shooter component responsible for firing projectiles.
            /// </summary>
            IShooter _shooter;
        
            /// <summary>
            /// Transform representing the turret object.
            /// </summary>
            Transform _turretTransform;
        
            /// <summary>
            /// Transform representing the spawn point for projectiles.
            /// </summary>
            Transform _spawnTransform;
        
            /// <summary>
            /// Reference to the jump behavior component for handling jump-related actions.
            /// </summary>
            IHealthBehaviour _healthBehaviour;
        
            /// <summary>
            /// Initializes a new instance of the TurretBuilder class with the specified TurretBehaviour instance.
            /// </summary>
            /// <param name="behaviour">The TurretBehaviour instance to build.</param>
            public TurretBuilder(TurretBehaviour behaviour)
            {
                _behaviour = behaviour;
            }
        
            /// <summary>
            /// Sets the input reader for the turret behavior.
            /// </summary>
            /// <param name="reader">The input reader.</param>
            /// <returns>The current TurretBuilder instance.</returns>
            public TurretBuilder WithInputReader(InputReader reader)
            {
                _inputReader = reader;
                return this;
            }
        
            /// <summary>
            /// Sets the rotator component for the turret behavior.
            /// </summary>
            /// <param name="rotator">The rotator component.</param>
            /// <returns>The current TurretBuilder instance.</returns>
            public TurretBuilder WithRotator(IRotator rotator)
            {
                _rotator = rotator;
                return this;
            }
        
            /// <summary>
            /// Sets the shooter component for the turret behavior.
            /// </summary>
            /// <param name="shooter">The shooter component.</param>
            /// <returns>The current TurretBuilder instance.</returns>
            public TurretBuilder WithShooter(IShooter shooter)
            {
                _shooter = shooter;
                return this;
            }
        
            /// <summary>
            /// Sets the turret transform for the turret behavior.
            /// </summary>
            /// <param name="turret">The turret transform.</param>
            /// <returns>The current TurretBuilder instance.</returns>
            public TurretBuilder WithTurretTransform(Transform turret)
            {
                _turretTransform = turret;
                return this;
            }
        
            /// <summary>
            /// Sets the spawn transform for the turret behavior.
            /// </summary>
            /// <param name="spawn">The spawn transform.</param>
            /// <returns>The current TurretBuilder instance.</returns>
            public TurretBuilder WithSpawnTransform(Transform spawn)
            {
                _spawnTransform = spawn;
                return this;
            }
        
            /// <summary>
            /// Sets the jump behavior component for the turret behavior.
            /// </summary>
            /// <param name="healthBehaviour">The jump behavior component.</param>
            /// <returns>The current TurretBuilder instance.</returns>
            public TurretBuilder WithJumpBehaviour(IHealthBehaviour healthBehaviour)
            {
                _healthBehaviour = healthBehaviour;
                return this;
            }
        
            /// <summary>
            /// Builds and initializes the TurretBehaviour instance.
            /// Throws an exception if any required component is not set.
            /// </summary>
            public void Build()
            {
                if (_inputReader == null) throw new InvalidOperationException("InputReader not set");
                if (_rotator == null) throw new InvalidOperationException("Rotator not set");
                if (_shooter == null) throw new InvalidOperationException("Shooter not set");
                if (_turretTransform == null) throw new InvalidOperationException("Turret Transform not set");
                if (_spawnTransform == null) throw new InvalidOperationException("Spawn Transform not set");
                if (_healthBehaviour == null) throw new InvalidOperationException("JumpBehaviour not set");
        
                _behaviour.Initialize(
                    _inputReader,
                    _rotator,
                    _shooter,
                    _turretTransform,
                    _spawnTransform,
                    _healthBehaviour
                );
            }
        }
        #endregion
    }
}
