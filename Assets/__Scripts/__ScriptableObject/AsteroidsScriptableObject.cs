using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/AsteroidsSO", fileName = "AsteroidsSO.asset")]
[System.Serializable]
public class AsteroidsScriptableObject : ScriptableObject
{
	[field: SerializeField] public float minVel { get; private set; } = 5;
	[field: SerializeField] public float maxVel { get; private set; } = 10;
	[field: SerializeField] public float maxAngularVel { get; private set; } = 10;
	[field: SerializeField] public int   initialSize { get; private set; } = 3;
	[field: SerializeField] public float asteroidScale { get; private set; } = 0.75f;
	[field: SerializeField] public int  numSmallerAsteroidsToSpawn { get; private set; } = 2;
	[field: SerializeField] public int[] pointsForAsteroidSize { get; private set; } = {0, 400, 200, 100};
	[field: SerializeField] public int initialAsteroids { get; private set; } = 3;

	[field: SerializeField] public GameObject[]  asteroidPrefabs { get; private set; }

    public GameObject GetAsteroidPrefab()
    {
        int ndx = Random.Range(0, asteroidPrefabs.Length);
        return asteroidPrefabs[ndx];
    }
}
