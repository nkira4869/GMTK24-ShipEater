using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWaveSpawner : MonoBehaviour
{
    public List<EnemyWave> enemyWaves; // List of waves to spawn
    public Camera mainCamera; // Reference to the main camera
    public float despawnOffset = 5f; // Distance beyond the camera view where enemies are despawned
    public float spawnOffset = 2f; // Offset to move spawn areas away from the edges of the screen

    private int currentWaveIndex = 0;
    private List<GameObject> activeEnemies = new List<GameObject>(); // Track active enemies

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        StartNextWave(); // Start the first wave
    }

    void Update()
    {
        // Check if all enemies in the current wave are dead
        if (activeEnemies.Count == 0 && currentWaveIndex < enemyWaves.Count)
        {
            StartNextWave();
        }

        // Check if any enemies should be despawned after going out of view
        DespawnOutOfViewEnemies();
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
                if (waveEnemy.spawnFromSide)
                {
                    SpawnEnemyFromSide(waveEnemy.enemyPrefab);
                }
                else
                {
                    SpawnEnemyFromTop(waveEnemy.enemyPrefab);
                }

                // Wait for the spawn rate before spawning the next enemy of the same type
                yield return new WaitForSeconds(waveEnemy.spawnRate);
            }

            // Wait for the interval before spawning the next enemy type in this wave
            yield return new WaitForSeconds(wave.intervalBetweenEnemies);
        }
    }

    // Method to spawn an enemy from the top of the screen
    void SpawnEnemyFromTop(GameObject enemyPrefab)
    {
        Vector3 spawnPosition = GetRandomTopSpawnPosition();

        // Set the appropriate rotation for enemies moving from top to bottom
        Quaternion rotation = Quaternion.Euler(0, 0, 180);

        GameObject spawnedEnemy = Instantiate(enemyPrefab, spawnPosition, rotation);
        activeEnemies.Add(spawnedEnemy);

        Health enemyHealth = spawnedEnemy.GetComponent<Health>();
        if (enemyHealth != null)
        {
            enemyHealth.onDeath += () => OnEnemyDeath(spawnedEnemy);
        }
    }

    // Method to spawn an enemy from the side of the screen
    void SpawnEnemyFromSide(GameObject enemyPrefab)
    {
        Vector3 spawnPosition = GetRandomSideSpawnPosition();

        // Determine the appropriate rotation based on the side the enemy is spawning from
        Quaternion rotation;
        bool spawnFromLeft = spawnPosition.x < mainCamera.transform.position.x;
        if (spawnFromLeft)
        {
            rotation = Quaternion.Euler(0, 0, -90); // Left to right
        }
        else
        {
            rotation = Quaternion.Euler(0, 0, 90); // Right to left
        }

        GameObject spawnedEnemy = Instantiate(enemyPrefab, spawnPosition, rotation);
        activeEnemies.Add(spawnedEnemy);

        Health enemyHealth = spawnedEnemy.GetComponent<Health>();
        if (enemyHealth != null)
        {
            enemyHealth.onDeath += () => OnEnemyDeath(spawnedEnemy);
        }
    }

    // Calculate a random position along the top edge of the camera view
    Vector3 GetRandomTopSpawnPosition()
    {
        float cameraHeight = mainCamera.orthographicSize * 2f;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        float xPosition = Random.Range(-cameraWidth / 2f + spawnOffset, cameraWidth / 2f - spawnOffset);
        float yPosition = mainCamera.transform.position.y + cameraHeight / 2f + spawnOffset;

        return new Vector3(xPosition, yPosition, 0f);
    }

    // Calculate a random position on either the left or right side of the camera view
    Vector3 GetRandomSideSpawnPosition()
    {
        float cameraHeight = mainCamera.orthographicSize * 2f;
        float yPosition = Random.Range(mainCamera.transform.position.y - cameraHeight / 2f + spawnOffset, mainCamera.transform.position.y + cameraHeight / 2f - spawnOffset);

        // Randomly choose to spawn from the left or right side
        bool spawnFromLeft = Random.value > 0.5f;
        float xPosition = spawnFromLeft ? mainCamera.transform.position.x - (mainCamera.orthographicSize * mainCamera.aspect) - spawnOffset
                                        : mainCamera.transform.position.x + (mainCamera.orthographicSize * mainCamera.aspect) + spawnOffset;

        return new Vector3(xPosition, yPosition, 0f);
    }

    // Method to check and despawn enemies that go out of view on the sides
    void DespawnOutOfViewEnemies()
    {
        float cameraHeight = mainCamera.orthographicSize * 2f;
        float cameraWidth = cameraHeight * mainCamera.aspect;
        float despawnYPosition = mainCamera.transform.position.y - cameraHeight / 2f - despawnOffset;
        float despawnXLeft = mainCamera.transform.position.x - cameraWidth / 2f - despawnOffset;
        float despawnXRight = mainCamera.transform.position.x + cameraWidth / 2f + despawnOffset;

        List<GameObject> enemiesToDespawn = new List<GameObject>();

        foreach (var enemy in activeEnemies)
        {
            if (enemy.transform.position.y < despawnYPosition || enemy.transform.position.x < despawnXLeft || enemy.transform.position.x > despawnXRight)
            {
                enemiesToDespawn.Add(enemy);
            }
        }

        foreach (var enemy in enemiesToDespawn)
        {
            activeEnemies.Remove(enemy);
            Destroy(enemy);
        }
    }

    // Callback for when an enemy dies
    void OnEnemyDeath(GameObject enemy)
    {
        activeEnemies.Remove(enemy);
        Destroy(enemy);
    }

    // Visualize the spawn and despawn areas using Gizmos
    private void OnDrawGizmos()
    {
        if (mainCamera == null)
            return;

        float cameraHeight = mainCamera.orthographicSize * 2f;
        float cameraWidth = cameraHeight * mainCamera.aspect;

        // Calculate the spawn and despawn positions
        Vector3 spawnPositionTop = new Vector3(0, mainCamera.transform.position.y + cameraHeight / 2f + spawnOffset, 0f);
        Vector3 despawnPositionBottom = new Vector3(0, mainCamera.transform.position.y - cameraHeight / 2f - despawnOffset, 0f);
        Vector3 despawnPositionLeft = new Vector3(mainCamera.transform.position.x - cameraWidth / 2f - despawnOffset, 0f, 0f);
        Vector3 despawnPositionRight = new Vector3(mainCamera.transform.position.x + cameraWidth / 2f + despawnOffset, 0f, 0f);

        // Draw the spawn area (top of the camera)
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(spawnPositionTop, new Vector3(cameraWidth - 2 * spawnOffset, 1f, 0f));

        // Draw the side spawn areas (left and right sides)
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(new Vector3(mainCamera.transform.position.x - cameraWidth / 2f - spawnOffset, mainCamera.transform.position.y, 0f), new Vector3(1f, cameraHeight - 2 * spawnOffset, 0f));
        Gizmos.DrawWireCube(new Vector3(mainCamera.transform.position.x + cameraWidth / 2f + spawnOffset, mainCamera.transform.position.y, 0f), new Vector3(1f, cameraHeight - 2 * spawnOffset, 0f));

        // Draw the despawn areas (bottom, left, and right)
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(despawnPositionBottom, new Vector3(cameraWidth, 1f, 0f));
        Gizmos.DrawWireCube(despawnPositionLeft, new Vector3(1f, cameraHeight, 0f));
        Gizmos.DrawWireCube(despawnPositionRight, new Vector3(1f, cameraHeight, 0f));
    }
}