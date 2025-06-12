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
      /// Reference to the rigidbody component of the player ship.
      /// </summary>
      private Rigidbody _rb;

      /// <summary>
      /// The movement speed of the player ship.
      /// </summary>
      private float _moveSpeed;
      
      /// <summary>
      ///
      /// </summary>
      private bool _isJumping = false;
      
      /// <summary>
      /// Indicates whether the player ship has been initialized.
      /// </summary>
      private bool _isInitialized = false;
      
      /// <summary>
      /// Reference to the jump behaviour component for managing player jump.
      /// </summary>
      private IJumpBehaviour _jumpBehaviour;
      
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
      private void Initialize(IMover mover, InputReader inputReader, float moveSpeed, Rigidbody rb, ITiltWithVelocity tiltWithVelocity, IJumpBehaviour jumpBehaviour)
      {
          _mover = mover;
          _inputReader = inputReader;
          _moveSpeed = moveSpeed;
          _rb = rb;
          _tiltWithVelocity = tiltWithVelocity;
          _inputReader.OnMoveEvent += OnMove;
      
          _jumpBehaviour = jumpBehaviour;
      
          _jumpBehaviour.OnDisappear += OnDisappear;
          _jumpBehaviour.OnReappear += OnReappear;
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
      private void OnDisappear()
      {
          if (_isJumping)
              return;
          _isJumping = true;
          _mover.Stop();
      }
      
      
      /// <summary>
      /// Handles the logic when the player ship dies.
      /// Stops movement, hides the ship, and raises the "Game Over" event.
      /// </summary>
      private void OnReappear()
      {
          _isJumping = false;
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
          _jumpBehaviour.OnReappear += OnReappear;
          _jumpBehaviour.OnDisappear += OnDisappear;
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
          _jumpBehaviour.OnReappear -= OnReappear;
          _jumpBehaviour.OnDisappear -= OnDisappear;
      }

      /// <summary>
      /// Unity's FixedUpdate method, called at a fixed time interval.
      /// Handles movement if the player ship is initialized and not dead.
      /// </summary>
      private void FixedUpdate()
      {
          if (!_isInitialized || _isJumping)
              return;
          HandleMovement();
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
          private IJumpBehaviour _jumpBehaviour;
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
          public PlayerBuilder WithJumpBehaviour(IJumpBehaviour jumpBehaviour)
          {
              _jumpBehaviour = jumpBehaviour;
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
                  _jumpBehaviour
              );
          }
      }

      #endregion
  }
}

