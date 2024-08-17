using System.Collections.Generic;
using UnityEngine;

public class ShipHexGridSystem : MonoBehaviour
{
    public float hexRadius = 1f; // The radius of each hex cell in world units
    private Dictionary<Vector2Int, bool> gridCells = new Dictionary<Vector2Int, bool>(); // A dictionary to store grid cells with axial coordinates
    private List<Vector2Int> directions = new List<Vector2Int>
    {
        new Vector2Int(1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(-1, 1),
        new Vector2Int(-1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(1, -1)
    };

    void Start()
    {
        // Start with the central cell occupied
        gridCells[new Vector2Int(0, 0)] = true;

        // Dynamically grow the grid with the outermost empty hex cells
        ExpandGrid();
    }

    // Converts axial grid coordinates to world position
    public Vector3 GetWorldPosition(Vector2Int axialCoords)
    {
        float x = hexRadius * (Mathf.Sqrt(3) * axialCoords.x + Mathf.Sqrt(3) / 2 * axialCoords.y);
        float y = hexRadius * (3f / 2 * axialCoords.y);
        return new Vector3(x, y, 0) + transform.position;
    }

    // Converts a world position to the nearest axial grid coordinates
    public Vector2Int GetAxialCoordinates(Vector3 worldPosition)
    {
        Vector3 position = worldPosition - transform.position;
        float q = (Mathf.Sqrt(3) / 3 * position.x - 1f / 3 * position.y) / hexRadius;
        float r = (2f / 3 * position.y) / hexRadius;
        return RoundToAxialCoordinates(new Vector2(q, r));
    }

    // Rounds the floating-point axial coordinates to the nearest hex grid point
    private Vector2Int RoundToAxialCoordinates(Vector2 axialCoords)
    {
        int q = Mathf.RoundToInt(axialCoords.x);
        int r = Mathf.RoundToInt(axialCoords.y);
        return new Vector2Int(q, r);
    }

    // Expands the grid to include the outermost layer of empty hex cells
    public void ExpandGrid()
    {
        HashSet<Vector2Int> outerCells = new HashSet<Vector2Int>();

        foreach (var cell in gridCells)
        {
            if (!cell.Value) continue; // Skip if the cell is not occupied

            // Check all neighbors of occupied cells
            foreach (var dir in directions)
            {
                Vector2Int neighbor = cell.Key + dir;
                if (!gridCells.ContainsKey(neighbor))
                {
                    outerCells.Add(neighbor); // Add empty neighbors to the outer layer
                }
            }
        }

        // Mark the outer cells as empty
        foreach (var cell in outerCells)
        {
            if (!gridCells.ContainsKey(cell))
            {
                gridCells[cell] = false; // False means the cell is empty
            }
        }
    }

    // Marks a cell as occupied
    public void OccupyCell(Vector2Int axialCoords)
    {
        if (gridCells.ContainsKey(axialCoords))
        {
            gridCells[axialCoords] = true; // Mark the cell as occupied
            ExpandGrid(); // Dynamically grow the grid after attaching a new part
        }
    }

    // Finds the nearest empty cell from a given world position
    public Vector2Int FindNearestEmptyCell(Vector3 worldPosition)
    {
        Vector2Int axialCoords = GetAxialCoordinates(worldPosition);

        if (gridCells.ContainsKey(axialCoords) && !gridCells[axialCoords])
        {
            return axialCoords; // Return the nearest empty cell if it's empty
        }

        // If the nearest cell is occupied, check the neighbors
        foreach (var dir in directions)
        {
            Vector2Int neighbor = axialCoords + dir;
            if (gridCells.ContainsKey(neighbor) && !gridCells[neighbor])
            {
                return neighbor;
            }
        }

        // As a fallback, return the nearest empty cell (you can implement a more complex search here if needed)
        foreach (var cell in gridCells)
        {
            if (!cell.Value)
            {
                return cell.Key;
            }
        }

        return axialCoords; // Fallback return
    }

    // Visualize the hex grid in the editor using Gizmos
    void OnDrawGizmos()
    {
        if (gridCells == null) return;

        foreach (var cell in gridCells)
        {
            Gizmos.color = cell.Value ? Color.red : Color.green; // Occupied cells are red, empty cells are green
            Gizmos.DrawWireSphere(GetWorldPosition(cell.Key), hexRadius * 0.9f); // Draw hex cells
        }
    }
}