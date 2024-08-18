using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    [Header("Layer Databases")]
    public ObjectDatabase backgroundDatabase;
    public ObjectDatabase levelCosmeticsDatabase;
    public ObjectDatabase obstaclesDatabase;
    public ObjectDatabase enemiesDatabase;

    [Header("Layer Settings")]
    public float spawnInterval = 2f; // Time between spawns for each layer
    public float scrollSpeed = 2f; // Speed at which objects move downward
    public Vector2 spawnAreaTop = new Vector2(-5f, 5f); // Defines the top area where objects are spawned (xMin, xMax)
    public float despawnYPosition = -10f; // Y position where objects will be despawned

    private Dictionary<GameObject, float> spawnedObjects = new Dictionary<GameObject, float>();

    void Start()
    {
        // Start spawning for each layer
        StartCoroutine(SpawnLayer(backgroundDatabase, "Background"));
        StartCoroutine(SpawnLayer(levelCosmeticsDatabase, "LevelCosmetics"));
        StartCoroutine(SpawnLayer(obstaclesDatabase, "Obstacles"));
        StartCoroutine(SpawnLayer(enemiesDatabase, "Enemies"));
    }

    void Update()
    {
        ScrollObjects(); // Continuously scroll all objects downward
        DespawnObjects(); // Despawn objects when they go off-screen
    }

    // Coroutine to handle spawning for a specific layer
    IEnumerator SpawnLayer(ObjectDatabase database, string layerName)
    {
        while (true)
        {
            SpawnObjectFromDatabase(database, layerName);
            yield return new WaitForSeconds(spawnInterval); // Wait for the next spawn
        }
    }

    // Method to spawn a random object from the given database at the top of the screen
    void SpawnObjectFromDatabase(ObjectDatabase database, string layerName)
    {
        if (database.objectsToSpawn.Count == 0) return;

        // Select a random object from the database
        GameObject objectToSpawn = database.objectsToSpawn[Random.Range(0, database.objectsToSpawn.Count)];

        // Determine the spawn position at the top of the screen within the defined area
        float spawnX = Random.Range(spawnAreaTop.x, spawnAreaTop.y);
        Vector3 spawnPosition = new Vector3(spawnX, transform.position.y, 0); // y position is the top of the screen

        // Instantiate the object and set its parent layer
        GameObject spawnedObject = Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
        spawnedObject.name = layerName; // Set the name to the layer for easy identification

        // Track the object for scrolling and despawning
        spawnedObjects.Add(spawnedObject, scrollSpeed);
    }

    // Scroll all spawned objects downward
    void ScrollObjects()
    {
        List<GameObject> objectsToScroll = new List<GameObject>(spawnedObjects.Keys);
        foreach (var obj in objectsToScroll)
        {
            obj.transform.position -= new Vector3(0, spawnedObjects[obj] * Time.deltaTime, 0);
        }
    }

    // Despawn objects when they go off-screen (below the despawn Y position)
    void DespawnObjects()
    {
        List<GameObject> objectsToRemove = new List<GameObject>();
        foreach (var obj in spawnedObjects.Keys)
        {
            if (obj.transform.position.y < despawnYPosition) // Off-screen below the defined despawn position
            {
                objectsToRemove.Add(obj);
            }
        }

        // Destroy and remove objects that are off-screen
        foreach (var obj in objectsToRemove)
        {
            spawnedObjects.Remove(obj);
            Destroy(obj);
        }
    }

    // Draw Gizmos to visualize the spawn and despawn areas
    private void OnDrawGizmos()
    {
        // Set the Gizmo color for the spawn area
        Gizmos.color = Color.green;
        // Draw the spawn area as a wire cube
        Gizmos.DrawWireCube(new Vector3((spawnAreaTop.x + spawnAreaTop.y) / 2, transform.position.y, 0), new Vector3(spawnAreaTop.y - spawnAreaTop.x, 0.5f, 0));

        // Set the Gizmo color for the despawn area
        Gizmos.color = Color.red;
        // Draw the despawn line below the screen
        Gizmos.DrawLine(new Vector3(spawnAreaTop.x, despawnYPosition, 0), new Vector3(spawnAreaTop.y, despawnYPosition, 0));
    }
}