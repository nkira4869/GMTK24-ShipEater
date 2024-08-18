using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CosmeticSpawner : MonoBehaviour
{
    public List<GameObject> cosmeticPrefabs; // List of cosmetic prefabs to randomly spawn
    public Vector2 spawnAreaTop = new Vector2(-5f, 5f); // Horizontal range for spawning (xMin, xMax)
    public float spawnInterval; // Time between spawns
    public float spawnIntervalMax = 5;
    public float spawnIntervalMin = 3;
    public float scrollSpeed = 1.5f; // Speed at which the cosmetic elements move downward
    public float minYPositionToDespawn = -10f; // Y position below which cosmetic elements will be despawned

    private List<GameObject> activeCosmetics = new List<GameObject>(); // Keep track of active cosmetic elements

    void Start()
    {
        spawnInterval = Random.Range(spawnIntervalMin, spawnIntervalMax);
        // Start the continuous spawning process
        StartCoroutine(SpawnCosmeticElements());
    }

    void Update()
    {
        ScrollCosmetics(); // Move the cosmetic elements downward
        DespawnCosmetics(); // Despawn cosmetics when they go off-screen
    }

    // Coroutine to continuously spawn cosmetic elements
    IEnumerator SpawnCosmeticElements()
    {
        while (true)
        {
            SpawnCosmetic();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    // Method to spawn a random cosmetic element at the top of the screen
    void SpawnCosmetic()
    {
        if (cosmeticPrefabs.Count == 0) return; // No prefabs available to spawn

        // Select a random prefab from the list
        GameObject prefabToSpawn = cosmeticPrefabs[Random.Range(0, cosmeticPrefabs.Count)];

        // Determine the spawn position within the defined horizontal range
        float spawnX = Random.Range(spawnAreaTop.x, spawnAreaTop.y);
        Vector3 spawnPosition = new Vector3(spawnX, transform.position.y, 0);

        // Instantiate the cosmetic element and add it to the active list
        GameObject spawnedCosmetic = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        activeCosmetics.Add(spawnedCosmetic);
    }

    // Method to move all active cosmetic elements downward
    void ScrollCosmetics()
    {
        foreach (var cosmetic in activeCosmetics)
        {
            cosmetic.transform.position -= new Vector3(0, scrollSpeed * Time.deltaTime, 0);
        }
    }

    // Method to despawn cosmetic elements when they go off-screen
    void DespawnCosmetics()
    {
        // Collect all objects that need to be despawned
        List<GameObject> cosmeticsToDespawn = new List<GameObject>();
        foreach (var cosmetic in activeCosmetics)
        {
            if (cosmetic.transform.position.y < minYPositionToDespawn)
            {
                cosmeticsToDespawn.Add(cosmetic);
            }
        }

        // Despawn the collected objects
        foreach (var cosmetic in cosmeticsToDespawn)
        {
            activeCosmetics.Remove(cosmetic);
            Destroy(cosmetic);
        }
    }

    // Optional: Visualize the spawn area in the Scene view for debugging
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(new Vector3((spawnAreaTop.x + spawnAreaTop.y) / 2, transform.position.y, 0), new Vector3(spawnAreaTop.y - spawnAreaTop.x, 1f, 1f));
    }
}