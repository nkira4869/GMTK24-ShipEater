using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyWave", menuName = "ScriptableObjects/EnemyWave", order = 1)]
public class EnemyWave : ScriptableObject
{
    [System.Serializable]
    public class WaveEnemy
    {
        public GameObject enemyPrefab; // The enemy prefab to spawn
        public int count; // Number of this enemy type to spawn
        public float spawnRate; // Delay between spawns of this enemy type
    }

    public List<WaveEnemy> enemiesToSpawn; // List of enemies and their counts for this wave
    public float intervalBetweenEnemies = 1f; // Delay between different enemy types
}
