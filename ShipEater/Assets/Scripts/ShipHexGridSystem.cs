using System.Collections.Generic;
using UnityEngine;

public class ShipHexGridSystem : MonoBehaviour
{
    public float hexRadius = 1f; // The radius of each hex cell in world units
    public LayerMask playerLayerMask; // Layer mask for detecting objects on the "Player" layer

    private Dictionary<Vector2Int, bool> gridCells = new Dictionary<Vector2Int, bool>(); // Stores grid cells with a flag indicating whether they are occupied
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
        // Initialize the grid with the central cell and mark it as occupied
        gridCells[new Vector2Int(0, 0)] = true;

        // Expand the grid to generate the first layer of empty cells
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

    // Checks if a grid cell is occupied by objects with the "Player" layer
    public bool IsCellOccupied(Vector2Int gridPosition)
    {
        Vector3 worldPosition = GetWorldPosition(gridPosition);
        Collider2D collider = Physics2D.OverlapCircle(worldPosition, hexRadius * 0.5f, playerLayerMask);
        return collider != null;
    }

    // Marks a cell as occupied
    public void MarkCellAsOccupied(Vector2Int gridPosition)
    {
        gridCells[gridPosition] = true;
        ExpandGrid(); // Expand the grid whenever a cell is occupied
    }

    // Expands the grid to ensure the outermost layer is all empty
    public void ExpandGrid()
    {
        HashSet<Vector2Int> newCells = new HashSet<Vector2Int>();

        foreach (var cell in gridCells)
        {
            if (!cell.Value) continue; // Skip if the cell is not occupied

            // Check all neighbors of occupied cells
            foreach (var dir in directions)
            {
                Vector2Int neighbor = cell.Key + dir;
                if (!gridCells.ContainsKey(neighbor))
                {
                    newCells.Add(neighbor); // Add new cells to track
                }
            }
        }

        // Add new outer cells to the grid and mark them as empty
        foreach (var cell in newCells)
        {
            gridCells[cell] = false;
        }
    }

    // Finds the nearest empty cell from a given world position
    public Vector2Int FindNearestEmptyCell(Vector3 worldPosition)
    {
        Vector2Int axialCoords = GetAxialCoordinates(worldPosition);

        // Return the nearest empty cell
        foreach (var cell in gridCells)
        {
            if (!cell.Value && !IsCellOccupied(cell.Key))
            {
                return cell.Key;
            }
        }

        return axialCoords; // Fallback return if no empty cells found
    }

    // Visualize the hex grid in the editor using Gizmos
    void OnDrawGizmos()
    {
        foreach (var cell in gridCells)
        {
            bool occupied = IsCellOccupied(cell.Key);
            Gizmos.color = occupied ? Color.red : Color.green; // Occupied cells are red, empty cells are green
            Gizmos.DrawWireSphere(GetWorldPosition(cell.Key), hexRadius * 0.9f); // Draw hex cells
        }
    }
}