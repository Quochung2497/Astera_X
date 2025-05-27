using System.Collections.Generic;
using Course.Attribute;
using Course.ObjectPool;
using UnityEngine;
using UnityEngine.Pool;
using Utility;
using Random = UnityEngine.Random;

namespace Course.Core
{
    public class AsteraXManager : PrivateSingleton<AsteraXManager>, IAsteraX
    {
           [Header("Set in Inspector")]
           [Tooltip("This sets the AsteroidsScriptableObject to be used throughout the game.")]
           [field: SerializeField]
           public AsteroidsScriptableObject asteroidsSO { get; private set; }

           public Transform asteroidParent { get; private set; }
           
           private GameObject _player;
           private List<Asteroid>           _asteroids;
           private const float MIN_ASTEROID_DIST_FROM_PLAYER_SHIP = 5;
           private List<IFactory<Asteroid>> _pools;
        
           public Asteroid GetRandomAsteroidFromPool()
           {
               var idx = Random.Range(0, _pools.Count);
               return _pools[idx].Get();
           }
           
           public void AddAsteroid(Asteroid asteroid)
           {
               if (_asteroids.IndexOf(asteroid) == -1)
               {
                   _asteroids.Add(asteroid);
               }
           }
           public void RemoveAsteroid(Asteroid asteroid)
           {
               if (_asteroids.IndexOf(asteroid) != -1)
               {
                   _asteroids.Remove(asteroid);
               }
           }
           private void SpawnParentAsteroid(int i)
           {
               var ast = GetRandomAsteroidFromPool();
               ast.gameObject.name = $"Asteroid_{i:00}";
               // Find a good location for the Asteroid to spawn
               Vector3 pos;
               do
               {
                   pos = ScreenBounds.TryGetInstance().RANDOM_ON_SCREEN_LOC;  
               } while ((pos - _player.transform.position).magnitude < MIN_ASTEROID_DIST_FROM_PLAYER_SHIP);
        
               ast.transform.position = pos;
               ast.InitializeCluster(asteroidsSO.initialSize);
           }
           
           private void Awake()
           {
               base.Awake();
               asteroidParent = transform;
               _player = GameObject.FindWithTag("Player");
               _pools = new List<IFactory<Asteroid>>();
               foreach (var prefabGO in asteroidsSO.asteroidPrefabs)
               {
                   var prefabAst = prefabGO.GetComponent<Asteroid>();
                   var asteroidPool  = new AsteroidPool(prefabAst, asteroidParent);
                   _pools.Add(new AsteroidFactory(asteroidPool));
               }
           }
        
           private void Start()
           {
               _asteroids = new List<Asteroid>();
               
               // Spawn the parent Asteroids, child Asteroids are taken care of by them
               for (int i = 0; i < asteroidsSO.initialAsteroids; i++)
               {
                   SpawnParentAsteroid(i);
               }
           }
        
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
    }
}

