using System.Collections;
using UnityEngine;

public class DebrisSpawner : MonoBehaviour
{
    public GameObject debrisPrefab; // Reference to the debris prefab
    public Vector2 topLeft; // The top-left corner of the spawn area
    public Vector2 bottomRight; // The bottom-right corner of the spawn area
    public float minSpawnInterval = 2f; // Minimum time between spawns
    public float maxSpawnInterval = 5f; // Maximum time between spawns

    void Start()
    {
        StartCoroutine(SpawnDebris());
    }

    IEnumerator SpawnDebris()
    {
        while (true)
        {
            // Wait for a random time between the min and max spawn intervals
            yield return new WaitForSeconds(Random.Range(minSpawnInterval, maxSpawnInterval));

            // Generate a random spawn position within the designated box area
            Vector3 spawnPosition = GetRandomPointInBox();

            // Instantiate the debris prefab at the calculated position
            Instantiate(debrisPrefab, spawnPosition, Quaternion.identity);
        }
    }

    Vector3 GetRandomPointInBox()
    {
        // Generate a random x and y within the defined box area
        float randomX = Random.Range(topLeft.x, bottomRight.x);
        float randomY = Random.Range(bottomRight.y, topLeft.y);

        return new Vector3(randomX, randomY, 0f); // Return the random position
    }

    // Draw the Gizmo to visualize the spawn area in the Editor
    void OnDrawGizmos()
    {
        // Set the Gizmo color
        Gizmos.color = Color.yellow;

        // Draw a wireframe box representing the spawn area
        Vector3 topLeftPosition = new Vector3(topLeft.x, topLeft.y, 0f);
        Vector3 bottomRightPosition = new Vector3(bottomRight.x, bottomRight.y, 0f);
        Vector3 size = bottomRightPosition - topLeftPosition;

        Gizmos.DrawWireCube(topLeftPosition + size / 2f, size);
    }
}