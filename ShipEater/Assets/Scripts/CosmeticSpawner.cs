using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CosmeticSpawner : MonoBehaviour
{
    public List<GameObject> cosmeticPrefabs; // List of cosmetic prefabs to randomly spawn
    public Vector2 spawnAreaTop = new Vector2(-5f, 5f); // Horizontal range for spawning (xMin, xMax)
    public float spawnInterval; // Time between spawns
    public float spawnIntervalMax = 5f;
    public float spawnIntervalMin = 3f;
    public float minScrollSpeed = 1f; // Minimum scroll speed for cosmetic elements
    public float maxScrollSpeed = 3f; // Maximum scroll speed for cosmetic elements
    public float minYPositionToDespawn = -10f; // Y position below which cosmetic elements will be despawned
    public float cosmeticLifetime = 10f; // Time before a cosmetic is destroyed
    public bool randomizeRotation = true; // Should the rotation be randomized?

    private List<CosmeticElement> activeCosmetics = new List<CosmeticElement>(); // Track active cosmetic elements and their scroll speeds

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

        // Instantiate the cosmetic element
        GameObject spawnedCosmetic = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);

        // Randomize the rotation if enabled
        if (randomizeRotation)
        {
            RandomizeCosmeticRotation(spawnedCosmetic);
        }

        // Randomize the scroll speed for this specific cosmetic element
        float randomizedScrollSpeed = Random.Range(minScrollSpeed, maxScrollSpeed);

        // Add the new cosmetic element and its scroll speed to the tracking list
        activeCosmetics.Add(new CosmeticElement(spawnedCosmetic, randomizedScrollSpeed));

        // Destroy the cosmetic after a set amount of time
        Destroy(spawnedCosmetic, cosmeticLifetime);
    }

    // Method to move all active cosmetic elements downward
    void ScrollCosmetics()
    {
        foreach (var cosmeticElement in activeCosmetics)
        {
            cosmeticElement.GameObject.transform.position -= new Vector3(0, cosmeticElement.ScrollSpeed * Time.deltaTime, 0);
        }
    }

    // Method to despawn cosmetic elements when they go off-screen
    void DespawnCosmetics()
    {
        // Collect all objects that need to be despawned
        List<CosmeticElement> cosmeticsToDespawn = new List<CosmeticElement>();
        foreach (var cosmeticElement in activeCosmetics)
        {
            if (cosmeticElement.GameObject.transform.position.y < minYPositionToDespawn)
            {
                cosmeticsToDespawn.Add(cosmeticElement);
            }
        }

        // Despawn the collected objects
        foreach (var cosmeticElement in cosmeticsToDespawn)
        {
            activeCosmetics.Remove(cosmeticElement);
            Destroy(cosmeticElement.GameObject);
        }
    }

    // Function to randomize the rotation of a cosmetic element without affecting its scroll direction
    void RandomizeCosmeticRotation(GameObject cosmetic)
    {
        // Randomly rotate the cosmetic on the z-axis while maintaining the downward scroll direction
        float randomRotationZ = Random.Range(0f, 360f);
        cosmetic.transform.rotation = Quaternion.Euler(0, 0, randomRotationZ);
    }

    // Optional: Visualize the spawn area in the Scene view for debugging
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(new Vector3((spawnAreaTop.x + spawnAreaTop.y) / 2, transform.position.y, 0), new Vector3(spawnAreaTop.y - spawnAreaTop.x, 1f, 1f));
    }

    // Helper class to track cosmetic elements and their scroll speeds
    private class CosmeticElement
    {
        public GameObject GameObject { get; }
        public float ScrollSpeed { get; }

        public CosmeticElement(GameObject gameObject, float scrollSpeed)
        {
            GameObject = gameObject;
            ScrollSpeed = scrollSpeed;
        }
    }
}