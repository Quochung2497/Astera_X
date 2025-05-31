using System.Collections.Generic;
using Course.Attribute;
using Course.ObjectPool;
using Course.ScriptableObject;
using Course.Utility;
using UnityEngine;
using UnityEngine.Pool;
using Utility;
using Random = UnityEngine.Random;

namespace Course.Core
{
    public class AsteraXManager : PrivateSingleton<AsteraXManager>, IAsteraX
    {
        #region Serialize Fields
        [Header("Set in Inspector")]
        [Tooltip("This sets the AsteroidsScriptableObject to be used throughout the game.")]
        [field: SerializeField]
        public AsteroidsConfig asteroidsSO { get; private set; }
        
        #endregion

        #region Public Properties

        /// <summary>
        /// The parent transform for all asteroid objects.
        /// </summary>
        public Transform asteroidParent { get; private set; }

        #endregion

        #region Private Fields

        /// <summary>
        /// Reference to the player GameObject.
        /// </summary>
        private GameObject _player;

        /// <summary>
        /// List of active asteroid objects in the game.
        /// </summary>
        private List<Asteroid> _asteroids;

        /// <summary>
        /// Minimum distance an asteroid must spawn away from the player's ship.
        /// </summary>
        private const float MIN_ASTEROID_DIST_FROM_PLAYER_SHIP = 5;

        /// <summary>
        /// List of asteroid factories used for pooling asteroid objects.
        /// </summary>
        private List<IFactory<Asteroid>> _pools;

        #endregion

        #region IAsteraX API

        /// <summary>
        /// Retrieves a random asteroid from the pool.
        /// </summary>
        /// <returns>A random asteroid instance.</returns>
        public Asteroid GetRandomAsteroidFromPool()
        {
            var idx = Random.Range(0, _pools.Count);
            return _pools[idx].Get();
        }

        /// <summary>
        /// Adds an asteroid to the active asteroid list if it is not already present.
        /// </summary>
        /// <param name="asteroid">The asteroid to add.</param>
        public void AddAsteroid(Asteroid asteroid)
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
        public void RemoveAsteroid(Asteroid asteroid)
        {
            if (_asteroids.IndexOf(asteroid) != -1)
            {
                _asteroids.Remove(asteroid);
            }
        }

        /// <summary>
        /// Finds a safe spawn position for an asteroid based on distance constraints.
        /// </summary>
        /// <param name="minDistAwayFromPlayer">Minimum distance from the player.</param>
        /// <param name="minDistAwayFromAsteroids">Minimum distance from other asteroids.</param>
        /// <param name="maxAttempts">Maximum number of attempts to find a valid position.</param>
        /// <param name="respawnDelay">Delay used to predict asteroid positions.</param>
        /// <returns>A safe spawn position for the asteroid.</returns>
        public Vector3 GetSafeSpawnPosition(
            float minDistAwayFromPlayer,
            float minDistAwayFromAsteroids,
            int maxAttempts = 50)
        {
            Vector3 pos = Vector3.zero;
            float sqPlayer = minDistAwayFromPlayer * minDistAwayFromPlayer;
            float sqAst = minDistAwayFromAsteroids * minDistAwayFromAsteroids;
            var playerT = _player.transform;

            for (int i = 0; i < maxAttempts; i++)
            {
                pos = ScreenBounds.TryGetInstance().RANDOM_ON_SCREEN_LOC;

                // Check if the position is far enough from the player
                if ((pos - playerT.position).sqrMagnitude < sqPlayer)
                    continue;

                // Check if the position is far enough from all asteroids
                bool tooClose = false;
                foreach (var a in _asteroids)
                {
                    if ((a.transform.position - pos).sqrMagnitude < sqAst)
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
            return ScreenBounds.TryGetInstance().RANDOM_ON_SCREEN_LOC;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Spawns a parent asteroid at a safe location.
        /// </summary>
        /// <param name="i">The index of the asteroid being spawned.</param>
        private void SpawnParentAsteroid(int i)
        {
            var ast = GetRandomAsteroidFromPool();
            ast.gameObject.name = $"Asteroid_{i:00}";

            // Find a safe location for the asteroid to spawn
            Vector3 pos;
            do
            {
                pos = ScreenBounds.TryGetInstance().RANDOM_ON_SCREEN_LOC;
            } while ((pos - _player.transform.position).magnitude < MIN_ASTEROID_DIST_FROM_PLAYER_SHIP);

            ast.transform.position = pos;
            ast.InitializeCluster(asteroidsSO.initialSize);
        }

        #endregion

        #region Unity Life Cycle

        /// <summary>
        /// Unity's Awake method. Initializes asteroid factories and sets up references.
        /// </summary>
        private void Awake()
        {
            base.Awake();
            asteroidParent = transform;
            _player = GameObject.FindWithTag("Player");
            _pools = new List<IFactory<Asteroid>>();
            foreach (var prefabGO in asteroidsSO.asteroidPrefabs)
            {
                var prefabAst = prefabGO.GetComponent<Asteroid>();
                var asteroidPool = new AsteroidPool(prefabAst, asteroidParent);
                _pools.Add(new AsteroidFactory(asteroidPool));
            }
        }

        /// <summary>
        /// Unity's Start method. Spawns initial parent asteroids.
        /// </summary>
        private void Start()
        {
            _asteroids = new List<Asteroid>();

            for (int i = 0; i < asteroidsSO.initialAsteroids; i++)
            {
                SpawnParentAsteroid(i);
            }
        }

        /// <summary>
        /// Unity's Update method. Spawns parent asteroids when the space key is pressed.
        /// </summary>
        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
            {
                for (int i = 0; i < asteroidsSO.initialAsteroids; i++)
                {
                    SpawnParentAsteroid(i);
                }
            }
        }

        #endregion
    }
}

