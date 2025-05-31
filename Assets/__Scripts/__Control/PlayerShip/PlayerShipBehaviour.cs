using System;
using System.Collections;
using Course.Attribute;
using Course.Core;
using Course.Utility;
using Course.Utility.Events;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Course.Control.Player
{ 
    [RequireComponent(typeof(Rigidbody))]
  public class PlayerShipBehaviour : MonoBehaviour
  {

      #region Serialize Fields

      /// <summary>
      /// The delay time before the player ship reappears after jumping.
      /// </summary>
      [SerializeField] float jumpDelay = 1f;

      /// <summary>
      /// The minimum separation distance from asteroids when determining a safe spawn position.
      /// </summary>
      [SerializeField] float minAsteroidSeparation = 3f;

      /// <summary>
      /// The minimum separation distance from the player when determining a safe spawn position.
      /// </summary>
      [SerializeField] float minPlayerSeparation = 5f;

      #endregion

      #region Private Properties

      /// <summary>
      /// Stores the movement input from the player.
      /// </summary>
      private Vector2 _moveInput;

      /// <summary>
      /// Stores the aiming input from the player.
      /// </summary>
      private Vector2 _aimInput;

      /// <summary>
      /// Reference to the input reader for handling player input events.
      /// </summary>
      private InputReader _inputReader;

      /// <summary>
      /// Reference to the mover component responsible for moving the player ship.
      /// </summary>
      private IMover _mover;

      /// <summary>
      /// Reference to the tilt logic component for tilting the player ship based on velocity.
      /// </summary>
      private ITiltWithVelocity _tiltWithVelocity;

      /// <summary>
      /// Reference to the jump behavior component for handling jump-related actions.
      /// </summary>
      private IHealthBehaviour _healthBehaviour;

      /// <summary>
      /// Reference to the AsteraX manager for accessing game-related settings and operations.
      /// </summary>
      private IAsteraX _asteraX;

      /// <summary>
      /// Array of renderers used to control the visibility of the player ship.
      /// </summary>
      private Renderer[] _renderers;

      /// <summary>
      /// Reference to the collider component of the player ship.
      /// </summary>
      private Collider _collider;

      /// <summary>
      /// Reference to the rigidbody component of the player ship.
      /// </summary>
      private Rigidbody _rb;

      /// <summary>
      /// The movement speed of the player ship.
      /// </summary>
      private float _moveSpeed;

      /// <summary>
      /// Indicates whether the player ship has been initialized.
      /// </summary>
      private bool _isInitialized = false;

      /// <summary>
      /// Event raised when the game state changes to "Game Over".
      /// </summary>
      private GameStateChangedEvent _gameStateGameOvervent;

      /// <summary>
      /// The amount of damage taken by the player ship when colliding with an asteroid.
      /// </summary>
      private int _damage;

      /// <summary>
      /// Flag indicating whether the player ship is immune to hits.
      /// </summary>
      private bool _hitImmune = false;
      #endregion

      #region Private Methods
      
      /// <summary>
      /// Initializes the player ship with the specified components and settings.
      /// </summary>
      /// <param name="mover">The mover component for handling movement.</param>
      /// <param name="inputReader">The input reader for handling player input.</param>
      /// <param name="moveSpeed">The movement speed of the player ship.</param>
      /// <param name="rb">The rigidbody component of the player ship.</param>
      /// <param name="tiltWithVelocity">The tilt logic component for tilting the ship.</param>
      private void Initialize(IMover mover, InputReader inputReader, float moveSpeed, Rigidbody rb, ITiltWithVelocity tiltWithVelocity, IHealthBehaviour healthBehaviour)
      {
          _mover = mover;
          _inputReader = inputReader;
          _moveSpeed = moveSpeed;
          _rb = rb;
          _tiltWithVelocity = tiltWithVelocity;
          _inputReader.OnMoveEvent += OnMove;
      
          _healthBehaviour = healthBehaviour;
          _asteraX = AsteraXManager.TryGetInstance();
          _renderers = GetComponentsInChildren<Renderer>();
          _collider = GetComponent<Collider>();
          _gameStateGameOvervent = new GameStateChangedEvent(GameState.gameOver);
      
          _healthBehaviour.OnDie += OnDie;
          _healthBehaviour.OnValueChanged += OnHealth;
          _isInitialized = true;
      }
      
      /// <summary>
      /// Handles the movement of the player ship based on input and velocity.
      /// </summary>
      private void HandleMovement()
      {
          _mover.Move(_rb, _moveInput, _moveSpeed);
          _tiltWithVelocity.Tilt(
              _moveInput,
              _rb.linearVelocity.magnitude,
              Time.deltaTime
          );
      }
      
      /// <summary>
      /// Updates the movement input based on player input events.
      /// </summary>
      /// <param name="delta">The movement input delta.</param>
      private void OnMove(Vector2 delta)
      {
          _moveInput = delta;
      }
      
      /// <summary>
      /// Handles the jump action when triggered by the jump behavior.
      /// </summary>
      private void OnHealth()
      {
          if (_healthBehaviour.IsDead)
              return;
          StartCoroutine(Jump());
      }
      
      /// <summary>
      /// Executes the jump action, including disappearing, waiting, teleporting, and reappearing.
      /// </summary>
      private IEnumerator Jump()
      {
          // disappear
          SetVisible(false);
      
          // wait
          yield return new WaitForSeconds(jumpDelay);
      
          // teleport to safe spot
          var safe = _asteraX.GetSafeSpawnPosition(minPlayerSeparation, minAsteroidSeparation, 20);
          transform.position = safe;
            
          // reappear
          SetVisible(true);
          _hitImmune = false;  
      }
      
      /// <summary>
      /// Sets the visibility of the player ship.
      /// </summary>
      /// <param name="v">True to make the ship visible; false to hide it.</param>
      private void SetVisible(bool v)
      {
          foreach (var r in _renderers) r.enabled = v;
          _collider.enabled = v;
      }
      
      /// <summary>
      /// Handles the logic when the player ship dies.
      /// Stops movement, hides the ship, and raises the "Game Over" event.
      /// </summary>
      private void OnDie()
      {
          _mover.Stop();
          SetVisible(false);
          EventBus<GameStateChangedEvent>.Raise(_gameStateGameOvervent);
      }
      #endregion

      #region Unity API

      /// <summary>
      /// Unity's OnEnable method, called when the object becomes enabled.
      /// Re-subscribes to input and jump behavior events if initialized.
      /// </summary>
      private void OnEnable()
      {
          if (!_isInitialized)
              return;
          _inputReader.OnMoveEvent += OnMove;
          _healthBehaviour.OnDie += OnDie;
      }

      /// <summary>
      /// Unity's OnDisable method, called when the object becomes disabled.
      /// Unsubscribes from input and jump behavior events if initialized.
      /// </summary>
      private void OnDisable()
      {
          if (!_isInitialized)
              return;
          _inputReader.OnMoveEvent -= OnMove;
          _healthBehaviour.OnDie -= OnDie;
      }

      /// <summary>
      /// Unity's FixedUpdate method, called at a fixed time interval.
      /// Handles movement if the player ship is initialized and not dead.
      /// </summary>
      private void FixedUpdate()
      {
          if (!_isInitialized || _healthBehaviour.IsDead)
              return;
          HandleMovement();
      }

      /// <summary>
      /// Unity's OnCollisionEnter method, called when the player ship collides with another object.
      /// Handles collision logic, including reducing jump behavior value if colliding with an asteroid.
      /// </summary>
      /// <param name="other">The collision data.</param>
      private void OnCollisionEnter(Collision other)
      {
          if (!_isInitialized || _healthBehaviour.IsDead)
              return;

          if (other.gameObject.layer == LayerNameProvider.GetLayer(LayerName.Asteroid))
          {
              _hitImmune = true;  
              _damage= _asteraX.asteroidsSO.damage;
              _healthBehaviour?.ChangeValue(-_damage);
          }
      }


      #endregion

      #region Builder

      /// <summary>
      /// Builder class for constructing and initializing a PlayerShipBehaviour instance.
      /// </summary>
      public class PlayerBuilder
      {
          private readonly PlayerShipBehaviour _behaviour;
          private IMover _mover;
          private InputReader _inputReader;
          private float _moveSpeed;
          private ITiltWithVelocity _tilt;
          private IHealthBehaviour _healthBehaviour;
          private Rigidbody _rb;

          /// <summary>
          /// Initializes a new instance of the PlayerBuilder class with the specified PlayerShipBehaviour instance.
          /// </summary>
          /// <param name="behaviour">The PlayerShipBehaviour instance to build.</param>
          public PlayerBuilder(PlayerShipBehaviour behaviour)
          {
              _behaviour = behaviour;
          }
          
          /// <summary>
          /// Sets the mover component for the player ship.
          /// </summary>
          /// <param name="mover">The mover component.</param>
          /// <returns>The current PlayerBuilder instance.</returns>
          public PlayerBuilder WithMover(IMover mover)
          {
              _mover = mover;
              return this;
          }
          
          /// <summary>
          /// Sets the input reader for the player ship.
          /// </summary>
          /// <param name="reader">The input reader.</param>
          /// <returns>The current PlayerBuilder instance.</returns>
          public PlayerBuilder WithInputReader(InputReader reader)
          {
              _inputReader = reader;
              return this;
          }
          
          /// <summary>
          /// Sets the movement speed for the player ship.
          /// </summary>
          /// <param name="speed">The movement speed.</param>
          /// <returns>The current PlayerBuilder instance.</returns>
          public PlayerBuilder WithMoveSpeed(float speed)
          {
              _moveSpeed = speed;
              return this;
          }
          
          /// <summary>
          /// Sets the tilt logic component for the player ship.
          /// </summary>
          /// <param name="tilt">The tilt logic component.</param>
          /// <returns>The current PlayerBuilder instance.</returns>
          public PlayerBuilder WithTiltLogic(ITiltWithVelocity tilt)
          {
              _tilt = tilt;
              return this;
          }
          
          /// <summary>
          /// Sets the rigidbody component for the player ship.
          /// </summary>
          /// <param name="rb">The rigidbody component.</param>
          /// <returns>The current PlayerBuilder instance.</returns>
          public PlayerBuilder WithRigidbody(Rigidbody rb)
          {
              _rb = rb;
              return this;
          }
          
          /// <summary>
          /// Sets the jump behaviour for the player ship.
          /// </summary>
          /// <param name="healthBehaviour"></param>
          /// <returns></returns>
          public PlayerBuilder WithJumpBehaviour(IHealthBehaviour healthBehaviour)
          {
              _healthBehaviour = healthBehaviour;
              return this;
          }
          
          /// <summary>
          /// Builds and initializes the PlayerShipBehaviour instance.
          /// Throws an exception if any required component is not set.
          /// </summary>
          public void Build()
          {
              if (_mover == null) throw new InvalidOperationException("Mover not set");
              if (_inputReader == null) throw new InvalidOperationException("InputReader not set");
              if (_tilt == null) throw new InvalidOperationException("TiltLogic not set");
              if (_rb == null) throw new InvalidOperationException("Rigidbody not set");
          
              _behaviour?.Initialize(
                  _mover,
                  _inputReader,
                  _moveSpeed,
                  _rb,
                  _tilt,
                  _healthBehaviour
              );
          }
      }

      #endregion
  }
}

