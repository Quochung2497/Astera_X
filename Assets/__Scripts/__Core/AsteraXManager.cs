using System.Collections.Generic;
using Course.Attribute;
using Course.Level;
using Course.ObjectPool;
using Course.ScriptableObject;
using Course.Utility;
using UnityEngine;
using Utility.DependencyInjection;
using Random = UnityEngine.Random;

namespace Course.Core
{
    public class AsteraXManager : PrivateSingleton<AsteraXManager>, IAsteraX, ISafeSpawnPositionProvider
    {
        #region Serialize Fields
        [Header("Set in Inspector")]
        [SerializeField] 
        private Transform anchor;
        
        #endregion

        #region Public Properties

        /// <summary>
        /// The parent transform for all asteroid objects.
        /// </summary>
        public Transform asteroidParent { get; private set; }

        public AsteroidsConfig asteroidsSO => _asteroidsConfig;

        #endregion

        #region Private Fields

        /// <summary>
        /// Reference to the player GameObject.
        /// </summary>
        private GameObject _player;

        /// <summary>
        /// List of active asteroid objects in the game.
        /// </summary>
        private readonly List<IAsteroid> _asteroids = new List<IAsteroid>();

        /// <summary>
        /// Minimum distance an asteroid must spawn away from the player's ship.
        /// </summary>
        private const float MIN_ASTEROID_DIST_FROM_PLAYER_SHIP = 5;

        /// <summary>
        /// List of asteroid factories used for pooling asteroid objects.
        /// </summary>
        private List<IFactory<Asteroid>> _pools;
        
        /// <summary>
        /// Our level driver. We subscribe to its OnLevelStarted and OnLevelCleared events.
        /// </summary>
        [Inject]
        private ILevel _levelDriver;

        [Inject]
        private AsteroidsConfig _asteroidsConfig;

        [Inject]
        private IScreenBounds _screenBounds;

        [Inject]
        private IGameStateReader _gameState;

        [Inject]
        private ILevelProgression _levelProgression;

        private bool _isSpawning = false;

        #endregion

        #region IAsteraX API

        /// <summary>
        /// Retrieves a random asteroid from the pool.
        /// </summary>
        /// <returns>A random asteroid instance.</returns>
        public IAsteroid GetRandomAsteroidFromPool(int size)
        {
            var idx = Random.Range(0, _pools.Count);
            IAsteroid ast = _pools[idx].Get();
            ast.SetSize(size);
            return ast;
        }

        /// <summary>
        /// Adds an asteroid to the active asteroid list if it is not already present.
        /// </summary>
        /// <param name="asteroid">The asteroid to add.</param>
        public void AddAsteroid(IAsteroid asteroid)
        {
            if (_asteroids.IndexOf(asteroid) == -1)
            {
                _asteroids.Add(asteroid);
            }
        }

        /// <summary>
        /// Removes an asteroid from the active asteroid list if it is present.
        /// </summary>
        /// <param name="asteroid">The asteroid to remove.</param>
        public void RemoveAsteroid(IAsteroid asteroid)
        {
            if (asteroid == null
                || (asteroid is Object unityObject && unityObject == null)
                || asteroid.gameObject == null)
                return;
            if (_asteroids.Remove(asteroid))
                _levelProgression.OnAsteroidRemoved(_asteroids.Count);
        }

        #endregion

        #region ISafeSpawnPositionProvider API

        /// <summary>
        /// Finds a landing position for the player's jump, with a random fallback.
        /// </summary>
        public Vector3 GetSafeSpawnPosition(
            float minDistAwayFromPlayer,
            float minDistAwayFromAsteroids,
            int maxAttempts = 50)
        {
            float minPlayerDistanceSquared = minDistAwayFromPlayer * minDistAwayFromPlayer;
            float minAsteroidDistanceSquared = minDistAwayFromAsteroids * minDistAwayFromAsteroids;
            var playerT = _player.transform;

            for (int i = 0; i < maxAttempts; i++)
            {
                Vector3 pos = _screenBounds.RANDOM_ON_SCREEN_LOC;
                if (SpawnPositionRules.IsTooCloseSquared(pos, playerT.position, minPlayerDistanceSquared))
                    continue;

                // Check if the position is far enough from all asteroids
                bool tooClose = false;
                foreach (var a in _asteroids)
                {
                    if (SpawnPositionRules.IsTooCloseSquared(a.transform.position, pos, minAsteroidDistanceSquared))
                    {
                        tooClose = true;
                        break;
                    }
                }
                if (tooClose)
                    continue;

                // Valid position found
                return pos;
            }

            // Fallback to a random position if no valid position is found
            return _screenBounds.RANDOM_ON_SCREEN_LOC;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Called each time the LevelBehaviour fires OnLevelStarted(levelNumber).
        /// We read the spawn rules for that level, clear any leftovers, then spawn exactly
        /// “parentCount” parents per rule, calling SpawnCluster(rule) each time.
        /// </summary>
        private void HandleLevelStarted()
        {
            if (_isSpawning) return;
            if (_gameState.CurrentState != GameState.level)
            {
                return;
            }

            try
            {
                _isSpawning = true;
                if (_levelDriver.CurrentSpawnRule is not { } rule)
                    return;

                // Spawn exactly rule.parentCount parents:
                for (int i = 0; i < rule.parentCount; i++)
                {
                    SpawnCluster(rule, i);
                }

                // If somehow we spawned nothing, still clear the level:
                if (_levelProgression.ShouldClearAfterSpawn(_asteroids.Count))
                {
                    _levelDriver.LevelCleared();
                }
            }
            finally
            {
                _isSpawning = false;
            }
        }
        
        private void HandleLevelCleared()
        {
            int levelNumber = _levelDriver.CurrentLevel;
        }
        
        private void SpawnCluster(SpawnRule rule, int parentIndex)
        {
   
            // a) Acquire one parent of size rule.parentSize
            IAsteroid parent = GetRandomAsteroidFromPool(rule.parentSize);
            parent.SetPointValue(rule.parentPoints);
            parent.SetDamageValue(rule.parentDamage);

            parent.gameObject.name = $"Asteroid_{parentIndex:00}";
            // b) Place it somewhere safe on screen
            if (_player == null || parent == null) return;
            Vector3 pos;
            do
            {
                pos = _screenBounds.RANDOM_ON_SCREEN_LOC;
            }
            while (SpawnPositionRules.IsTooClose(pos, _player.transform.position, MIN_ASTEROID_DIST_FROM_PLAYER_SHIP));

            parent.transform.position = pos;
            parent.Setup(rule.parentHealth); 

            parent.SpawnDescendants(
                rule.childSize,
                rule.childCount,
                rule.childHealth,
                rule.childPoints,
                rule.childDamage,
                rule.grandchildSize,
                rule.grandchildCount,
                rule.grandchildHealth,
                rule.grandchildPoints,
                rule.grandchildDamage
            );
        }

        #endregion

        #region Unity Life Cycle

        /// <summary>
        /// Unity's Awake method. Initializes asteroid factories and sets up references.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            asteroidParent = anchor;
            _player = GameObject.FindWithTag("Player");
            _pools = new List<IFactory<Asteroid>>();
            
            var prefabs = asteroidsSO.GetPrefabs();  
            foreach (var prefabGO in prefabs)
            {
                var prefabAst = prefabGO.GetComponent<Asteroid>();
                var asteroidPool = new AsteroidPool(prefabAst, asteroidParent, _screenBounds, _asteroidsConfig);
                _pools.Add(new AsteroidFactory(asteroidPool));
            }
            // TODO: Read the starting level from game settings.
            _levelDriver.Initialize(1);
        }

        private void OnEnable()
        {
            if (_levelDriver != null)
            {
                _levelDriver.OnLevelStarted += HandleLevelStarted;
                _levelDriver.OnLevelCleared += HandleLevelCleared;
            }
        }

        private void OnDisable()
        {
            if (_levelDriver != null)
            {
                _levelDriver.OnLevelStarted -= HandleLevelStarted;
                _levelDriver.OnLevelCleared -= HandleLevelCleared;
            }
        }

        private void LateUpdate()
        {
            if (!_levelProgression.HasPendingClear) return;

            // Let every collision in this frame finish, including a possible player death.
            if (_levelProgression.TryResolveClear(_gameState.CurrentState, _asteroids.Count))
                _levelDriver.LevelCleared();
        }

        #endregion
    }
}

