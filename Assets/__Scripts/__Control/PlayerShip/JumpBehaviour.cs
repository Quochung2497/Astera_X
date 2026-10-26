using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Course.Attribute;
using Course.Control;
using Course.Control.Player;
using Course.Core;
using Course.Utility.Events;
using UnityEngine;

public class JumpBehaviour : MonoBehaviour,IJumpBehaviour
{
    #region Serialized Fields

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
    
    [SerializeField] float immuneTime = 1f;

    #endregion

    #region Private Fields

    private IHealthBehaviour _healthBehaviour;
    private IAsteraX        _asteraX;
    private Renderer[]      _renderers;
    private Collider        _collider;
    private List<Renderer> _renderersCache;
    private bool            _isJumping = false;
    private bool            _isInitialized = false;
    private bool            _healthEventsBound;
    /// <summary>
    /// The amount of damage taken by the player ship when colliding with an asteroid.
    /// </summary>
    private int _damage;
    
    /// <summary>
    /// Event raised when the game state changes to "Game Over".
    /// </summary>
    private GameStateChangedEvent _gameStateGameOverevent;
    #endregion

    #region Public Methods

    public void Initialize(IHealthBehaviour healthBehaviour,IAsteraX asteraX)
    {
        _healthBehaviour = healthBehaviour 
                           ?? throw new ArgumentNullException(nameof(healthBehaviour));

        _asteraX = asteraX;
        _collider  = GetComponent<Collider>();
        _gameStateGameOverevent = new GameStateChangedEvent(GameState.gameOver);

        _isInitialized = true;
        if (isActiveAndEnabled)
            SubscribeHealthEvents();
    }

    #endregion

    #region IJumpBehaviour Implementation

    public event Action OnDisappear;
    public event Action OnReappear;
    
    #endregion
    
    private void OnEnable()
    {
        SubscribeHealthEvents();
    }

    private void OnDisable()
    {
        UnsubscribeHealthEvents();
    }

    private void SubscribeHealthEvents()
    {
        if (!_isInitialized || _healthEventsBound)
            return;

        _healthBehaviour.OnValueChanged += HandleOnHealthChanged;
        _healthBehaviour.OnDie += HandleOnDeath;
        _healthEventsBound = true;
    }

    private void UnsubscribeHealthEvents()
    {
        if (!_healthEventsBound)
            return;

        _healthBehaviour.OnValueChanged -= HandleOnHealthChanged;
        _healthBehaviour.OnDie -= HandleOnDeath;
        _healthEventsBound = false;
    }

    private void HandleOnHealthChanged()
    {
        if (_isJumping || GameManager.TryGetInstance().currentStates != GameState.level)
            return;

        StartCoroutine(JumpSequence());
    }

    private IEnumerator JumpSequence()
    {
        _isJumping = true;
        
        if (_healthBehaviour.IsDead)
        {
            _isJumping = false;
            yield break;
        }
        
        OnDisappear?.Invoke();
        
        SetVisible(false);
        
        yield return new WaitForSeconds(jumpDelay);

        Vector3 safePosition = _asteraX.GetSafeSpawnPosition(
            minPlayerSeparation,
            minAsteroidSeparation,
            20
        );
        transform.position = safePosition;

        SetVisible(true);

        OnReappear?.Invoke();

        yield return new WaitForSeconds(immuneTime);
        _isJumping = false;
    }

    private void HandleOnDeath()
    {
        OnDisappear?.Invoke();
        SetVisible(false);
        EventBus<GameStateChangedEvent>.Raise(_gameStateGameOverevent);
    }
    
    private void SetVisible(bool visible)
    {
        _renderers = GetComponentsInChildren<Renderer>(); //TODO: Optimize this if possible
        foreach (var r in _renderers) r.enabled = visible;
        if (_collider != null) _collider.enabled = visible;
    }
    
    
    /// <summary>
    /// Unity's OnCollisionEnter method, called when the player ship collides with another object.
    /// Handles collision logic, including reducing jump behavior value if colliding with an asteroid.
    /// </summary>
    /// <param name="other">The collision data.</param>
    private void OnCollisionEnter(Collision other)
    {
        if (!_isInitialized)
            return;

        if (other.gameObject.layer == LayerNameProvider.GetLayer(LayerName.Asteroid))
        {
            var ast = other.gameObject.GetComponent<Asteroid>();
            if (ast == null) return;

            _damage = ast.damage;
            if (!ast.TryDestroyFromPlayerCollision()) return;

            // Finish the asteroid before Game Over can deactivate the asteroid group.
            if (!_isJumping && !_healthBehaviour.IsDead)
                _healthBehaviour.ChangeValue(-_damage);
        }
    }
}
