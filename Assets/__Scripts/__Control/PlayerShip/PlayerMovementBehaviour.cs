using System;
using Course.Utility;
using UnityEngine;

namespace Course.Control.Player
{
    [RequireComponent(typeof(Rigidbody))]
  public class PlayerMovementBehaviour : MonoBehaviour
  { 
      // Cache
    private Vector2 _moveInput;
    private Vector2 _aimInput;
    private InputReader _inputReader;
    private IMover _mover;
    private ITiltWithVelocity _tiltWithVelocity;
    private Rigidbody _rb;
    private float _moveSpeed;
    private bool _isInitialized = false;
    
    private void Initialize(IMover mover,InputReader inputReader, float moveSpeed, Rigidbody rb, ITiltWithVelocity tiltWithVelocity)
    {
        _mover          = mover;
        _inputReader     = inputReader;
        _moveSpeed       = moveSpeed;
        _rb            = rb;
        _tiltWithVelocity  = tiltWithVelocity;
        _inputReader.OnMoveEvent += OnMove;
        _isInitialized   = true;
    }

    private void OnEnable()
    {
        if(!_isInitialized)
            return;
        _inputReader.OnMoveEvent += OnMove;
    }

    private void OnDisable()
    {
        if(!_isInitialized)
            return;
        _inputReader.OnMoveEvent -= OnMove;
    }

    private void FixedUpdate()
    {
        if(!_isInitialized)
            return;
        HandleMovement();
    }
    
    private void HandleMovement()
    {
        _mover.Move(_rb, _moveInput, _moveSpeed);
        _tiltWithVelocity.Tilt(
            _moveInput,
            _rb.linearVelocity.magnitude,
            Time.deltaTime
        );
    }
    
    private void OnMove(Vector2 delta)
    {
        _moveInput = delta;
    }
    
      
    public class PlayerBuilder
    {
        private readonly PlayerMovementBehaviour _behaviour;
    
        private IMover              _mover;
        private InputReader         _inputReader;
        private float               _moveSpeed;
        private ITiltWithVelocity   _tilt;
        private Rigidbody         _rb;
    
        public PlayerBuilder(PlayerMovementBehaviour behaviour)
        {
            _behaviour = behaviour;
        }
    
        public PlayerBuilder WithMover(IMover mover)
        {
            _mover = mover;
            return this;
        }
    
        public PlayerBuilder WithInputReader(InputReader reader)
        {
            _inputReader = reader;
            return this;
        }
    
        public PlayerBuilder WithMoveSpeed(float speed)
        {
            _moveSpeed = speed;
            return this;
        }
    
        public PlayerBuilder WithTiltLogic(ITiltWithVelocity tilt)
        {
            _tilt = tilt;
            return this;
        }
        
        public PlayerBuilder WithRigidbody(Rigidbody rb)
        {
            _rb = rb;
            return this;
        }
    
        public void Build()
        {
            if (_mover      == null) throw new InvalidOperationException("Mover not set");
            if (_inputReader== null) throw new InvalidOperationException("InputReader not set");
            if (_tilt       == null) throw new InvalidOperationException("TiltLogic not set");
            if (_rb         == null) throw new InvalidOperationException("Rigidbody not set");
            
            _behaviour?.Initialize(
                _mover,
                _inputReader,
                _moveSpeed,
                _rb,
                _tilt
            );
        }
    }
  }
}

