using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWaveSpawner : MonoBehaviour
{
    public List<EnemyWave> enemyWaves; // List of waves to spawn
    public Transform[] spawnPoints; // Spawn points where enemies will appear

    private int currentWaveIndex = 0;
    private List<GameObject> activeEnemies = new List<GameObject>(); // Track active enemies

    void Start()
    {
        //StartNextWave(); // Start the first wave
    }

    void Update()
    {
        // Check if all enemies in the current wave are dead
        if (activeEnemies.Count == 0 && currentWaveIndex < enemyWaves.Count)
        {
            StartNextWave();
        }
    }

    // Method to start spawning a new wave
    void StartNextWave()
    {
        if (currentWaveIndex >= enemyWaves.Count) return; // No more waves to spawn

        StartCoroutine(SpawnWave(enemyWaves[currentWaveIndex]));
        currentWaveIndex++;
    }

    // Coroutine to spawn all enemies in a wave
    IEnumerator SpawnWave(EnemyWave wave)
    {
        foreach (var waveEnemy in wave.enemiesToSpawn)
        {
            for (int i = 0; i < waveEnemy.count; i++)
            {
                SpawnEnemy(waveEnemy.enemyPrefab);

                // Wait for the spawn rate before spawning the next enemy of the same type
                yield return new WaitForSeconds(waveEnemy.spawnRate);
            }

            // Wait for the interval before spawning the next enemy type in this wave
            yield return new WaitForSeconds(wave.intervalBetweenEnemies);
        }
    }

    // Method to spawn a single enemy and track it
    void SpawnEnemy(GameObject enemyPrefab)
    {
        if (spawnPoints.Length == 0) return;

        // Choose a random spawn point
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Instantiate the enemy and track it
        GameObject spawnedEnemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        activeEnemies.Add(spawnedEnemy);

        // Subscribe to the enemy's death event (assuming an Enemy script handles this)
        spawnedEnemy.GetComponent<Health>().onDeath += () => OnEnemyDeath(spawnedEnemy);
    }

    // Callback for when an enemy dies
    void OnEnemyDeath(GameObject enemy)
    {
        activeEnemies.Remove(enemy);
        Destroy(enemy);
    }
}