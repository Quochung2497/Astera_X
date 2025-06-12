using System;
using System.Collections;
using Course.Attribute;
using Course.Control.Player;
using Course.Core;
using Course.Utility.Events;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

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
    private bool            _isJumping = false;
    private bool            _isInitialized = false;
    /// <summary>
    /// The amount of damage taken by the player ship when colliding with an asteroid.
    /// </summary>
    private int _damage;

    /// <summary>
    /// Flag indicating whether the player ship is immune to hits.
    /// </summary>
    private bool _hitImmune = false;
    
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
        _renderers = GetComponentsInChildren<Renderer>();
        _collider  = GetComponent<Collider>();
        _gameStateGameOverevent = new GameStateChangedEvent(GameState.gameOver);

        _healthBehaviour.OnValueChanged += HandleOnHealthChanged;
        _healthBehaviour.OnDie += HandleOnDeath;
        _isInitialized = true;
    }

    #endregion

    #region IJumpBehaviour Implementation

    public event Action OnDisappear;
    public event Action OnReappear;
    
    #endregion
    
    private void OnEnable()
    {
        if (_healthBehaviour == null && !_isInitialized)
            return;
        Debug.Assert(_healthBehaviour != null, nameof(_healthBehaviour) + " != null");
        _healthBehaviour.OnValueChanged += HandleOnHealthChanged;
        _healthBehaviour.OnDie += HandleOnDeath;
    }

    private void OnDisable()
    {
        if (_healthBehaviour == null && !_isInitialized)
            return;
        Debug.Assert(_healthBehaviour != null, nameof(_healthBehaviour) + " != null");
        _healthBehaviour.OnValueChanged -= HandleOnHealthChanged;
        _healthBehaviour.OnDie -= HandleOnDeath;
    }

    private void HandleOnHealthChanged()
    {
        if (_isJumping)
            return;

        StartCoroutine(JumpSequence());
    }

    private IEnumerator JumpSequence()
    {
        _isJumping = true;

        OnDisappear?.Invoke();

        SetVisible(false);

        if (_healthBehaviour.IsDead)
        {
            _isJumping = false;
            yield break;
        }

        yield return new WaitForSeconds(jumpDelay);

        Vector3 safePosition = _asteraX.GetSafeSpawnPosition(
            minPlayerSeparation,
            minAsteroidSeparation,
            20
        );
        transform.position = safePosition;

        SetVisible(true);

        OnReappear?.Invoke();

        _isJumping = false;
        yield return new WaitForSeconds(immuneTime);
        _hitImmune = false;
    }

    private void HandleOnDeath()
    {
        SetVisible(false);
        EventBus<GameStateChangedEvent>.Raise(_gameStateGameOverevent);
    }
    
    private void SetVisible(bool visible)
    {
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
        if (!_isInitialized || _isJumping || _hitImmune)
            return;

        if (other.gameObject.layer == LayerNameProvider.GetLayer(LayerName.Asteroid))
        {
            _hitImmune = true;
            var ast = other.gameObject.GetComponent<Asteroid>();
            _damage = ast.damage;
            _healthBehaviour?.ChangeValue(-_damage);
        }
    }
}
