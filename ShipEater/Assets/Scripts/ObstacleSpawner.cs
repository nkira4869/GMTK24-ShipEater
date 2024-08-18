using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public List<GameObject> obstaclePrefabs; // List of obstacle prefabs to spawn
    public Vector2 spawnAreaTop = new Vector2(-5f, 5f); // Horizontal range for spawning (xMin, xMax)
    public float spawnInterval = 3f; // Time between spawns
    public float scrollSpeed = 2f; // Speed at which obstacles move downward
    public int maxActiveObstacles = 5; // Maximum number of obstacles allowed on-screen at a time
    public float minYPositionToDespawn = -10f; // Y position below which obstacles will be despawned

    private List<GameObject> activeObstacles = new List<GameObject>(); // Track currently active obstacles
    private List<GameObject> availableObstacles; // Track obstacles available to spawn

    void Start()
    {
        availableObstacles = new List<GameObject>(obstaclePrefabs); // Initialize available obstacles list
        StartCoroutine(SpawnObstacles());
    }

    void Update()
    {
        ScrollObstacles(); // Move the obstacles downward
        DespawnObstacles(); // Despawn obstacles when they go off-screen
    }

    // Coroutine to continuously spawn obstacles
    IEnumerator SpawnObstacles()
    {
        while (true)
        {
            if (activeObstacles.Count < maxActiveObstacles && availableObstacles.Count > 0)
            {
                SpawnObstacle();
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    // Method to spawn a random obstacle from the available list
    void SpawnObstacle()
    {
        if (availableObstacles.Count == 0) return; // No available obstacles to spawn

        // Select a random obstacle from the available list
        GameObject prefabToSpawn = availableObstacles[Random.Range(0, availableObstacles.Count)];

        // Determine the spawn position within the defined horizontal range
        float spawnX = Random.Range(spawnAreaTop.x, spawnAreaTop.y);
        Vector3 spawnPosition = new Vector3(spawnX, transform.position.y, 0);

        // Instantiate the obstacle and add it to the active list
        GameObject spawnedObstacle = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        activeObstacles.Add(spawnedObstacle);

        // Remove the spawned obstacle from the available list
        availableObstacles.Remove(prefabToSpawn);
    }

    // Method to move all active obstacles downward
    void ScrollObstacles()
    {
        foreach (var obstacle in activeObstacles)
        {
            obstacle.transform.position -= new Vector3(0, scrollSpeed * Time.deltaTime, 0);
        }
    }

    // Method to despawn obstacles when they go off-screen
    void DespawnObstacles()
    {
        List<GameObject> obstaclesToDespawn = new List<GameObject>();

        foreach (var obstacle in activeObstacles)
        {
            if (obstacle.transform.position.y < minYPositionToDespawn)
            {
                obstaclesToDespawn.Add(obstacle);
            }
        }

        // Despawn and re-enable the collected obstacles
        foreach (var obstacle in obstaclesToDespawn)
        {
            activeObstacles.Remove(obstacle);
            availableObstacles.Add(obstacle); // Make it available for future spawns
            Destroy(obstacle); // Destroy the object (or use pooling if needed)
        }
    }

    // Optional: Visualize the spawn area in the Scene view for debugging
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(new Vector3((spawnAreaTop.x + spawnAreaTop.y) / 2, transform.position.y, 0), new Vector3(spawnAreaTop.y - spawnAreaTop.x, 1f, 1f));
    }
}