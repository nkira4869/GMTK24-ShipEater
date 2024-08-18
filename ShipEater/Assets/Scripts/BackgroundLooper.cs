using System.Collections.Generic;
using UnityEngine;

public class BackgroundLooper : MonoBehaviour
{
    public GameObject backgroundTilePrefab; // Prefab for a single background tile
    public int numberOfTiles = 3; // Number of background tiles to loop
    public float tileHeight = 10f; // Height of each background tile
    public float scrollSpeed = 2f; // Speed at which the background tiles scroll downward

    private List<GameObject> backgroundTiles = new List<GameObject>();

    void Start()
    {
        // Spawn and position the background tiles initially
        for (int i = 0; i < numberOfTiles; i++)
        {
            GameObject tile = Instantiate(backgroundTilePrefab, new Vector3(0, i * tileHeight, 0), Quaternion.identity);
            tile.transform.SetParent(transform); // Parent to the BackgroundLooper object
            backgroundTiles.Add(tile);
        }
    }

    void Update()
    {
        // Scroll the background tiles downward based on scrollSpeed
        foreach (var tile in backgroundTiles)
        {
            tile.transform.position -= new Vector3(0, Time.deltaTime * scrollSpeed, 0);
        }

        // Check if any tile is out of view and reposition it to the top
        foreach (var tile in backgroundTiles)
        {
            if (tile.transform.position.y < -tileHeight)
            {
                RepositionTile(tile);
            }
        }
    }

    // Reposition a tile at the top to loop the background
    void RepositionTile(GameObject tile)
    {
        // Find the highest y-position among all tiles
        float highestY = float.MinValue;
        foreach (var bgTile in backgroundTiles)
        {
            if (bgTile.transform.position.y > highestY)
            {
                highestY = bgTile.transform.position.y;
            }
        }

        // Reposition the tile just above the highest tile
        tile.transform.position = new Vector3(tile.transform.position.x, highestY + tileHeight, tile.transform.position.z);
    }

    // Optional: Visualize the background tiles in the Scene view for debugging
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        for (int i = 0; i < numberOfTiles; i++)
        {
            Gizmos.DrawWireCube(new Vector3(0, i * tileHeight, 0), new Vector3(10, tileHeight, 1)); // Adjust the size as needed
        }
    }
}