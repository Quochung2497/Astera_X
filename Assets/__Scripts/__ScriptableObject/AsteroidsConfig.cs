using UnityEngine;

namespace Course.ScriptableObject
{
    [CreateAssetMenu(menuName = "Scriptable Objects/AsteroidsSO", fileName = "AsteroidsSO.asset")]
    public class AsteroidsConfig : UnityEngine.ScriptableObject
    {
        /// <summary>
        /// Minimum velocity for asteroid movement.
        /// Serialized field to allow customization in the Unity Inspector.
        /// </summary>
        [field: SerializeField] public float minVel { get; private set; } = 5;
    
        /// <summary>
        /// Maximum velocity for asteroid movement.
        /// Serialized field to allow customization in the Unity Inspector.
        /// </summary>
        [field: SerializeField] public float maxVel { get; private set; } = 10;
    
        /// <summary>
        /// Maximum angular velocity for asteroid rotation.
        /// Serialized field to allow customization in the Unity Inspector.
        /// </summary>
        [field: SerializeField] public float maxAngularVel { get; private set; } = 10;
    
        /// <summary>
        /// Scale factor for asteroid size.
        /// Serialized field to allow customization in the Unity Inspector.
        /// </summary>
        [field: SerializeField] public float asteroidScale { get; private set; } = 0.75f;
        
        /// <summary>
        /// Damage dealt by asteroids when they collide with the player.
        /// </summary> 
        [field: SerializeField] public int damage { get; private set; } = 1;
    
        /// <summary>
        /// Number of smaller asteroids to spawn when an asteroid is destroyed.
        /// Serialized field to allow customization in the Unity Inspector.
        /// </summary>
        [field: SerializeField] public int numSmallerAsteroidsToSpawn { get; private set; } = 2;
    
        /// <summary>
        /// Initial size of asteroids.
        /// Serialized field to allow customization in the Unity Inspector.
        /// </summary>
        [field: SerializeField] public int initialSize { get; private set; } = 3;
    
        /// <summary>
        /// Number of asteroids to spawn at the start of the game.
        /// Serialized field to allow customization in the Unity Inspector.
        /// </summary>
        [field: SerializeField] public int initialAsteroids { get; private set; } = 3;
    
        /// <summary>
        /// Points awarded for destroying asteroids of different sizes.
        /// Serialized field to allow customization in the Unity Inspector.
        /// </summary>
        [field: SerializeField] public int[] pointsForAsteroidSize { get; private set; } = {0, 400, 200, 100};
    
        /// <summary>
        /// Array of asteroid prefabs used for instantiation.
        /// Serialized field to allow customization in the Unity Inspector.
        /// </summary>
        [field: SerializeField] public GameObject[] asteroidPrefabs { get; private set; }
        
        
        [Header("Health by Size")]
        [Tooltip("Number of hit‐points for an asteroid of a given size.\n"
                 + "Index 1 = health for size 1 (small), index 2 = health for size 2 (medium), etc.")]
        [field: SerializeField]
        public int[] healthForAsteroidSize { get; private set; } = { 0, 1, 2, 3 };
    
        /// <summary>
        /// Retrieves a random asteroid prefab from the array of prefabs.
        /// </summary>
        /// <returns>A randomly selected asteroid prefab.</returns>
        public GameObject GetAsteroidPrefab()
        {
            int ndx = Random.Range(0, asteroidPrefabs.Length);
            return asteroidPrefabs[ndx];
        }
    }
}

