using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DynamicHexGrid : MonoBehaviour
{
    public int gridRadius = 5; // The radius of the hex grid
    public float hexSize = 1f; // The size of each hexagon
    public GameObject ship; // The player's ship that controls the grid

    private Dictionary<Vector2Int, Hex> hexes = new Dictionary<Vector2Int, Hex>();
    private HashSet<Vector2Int> occupiedCells = new HashSet<Vector2Int>(); // Track occupied cells
    private HashSet<Vector2Int> reservedCells = new HashSet<Vector2Int>(); // Track reserved cells

    public UnityEvent<float> OnGridScaled;

    void Start()
    {
        if (OnGridScaled == null)
            OnGridScaled = new UnityEvent<float>();

        GenerateHexGrid();
        MarkCenterCellAsOccupied();
    }

    // Generates the hex grid using axial coordinates
    void GenerateHexGrid()
    {
        for (int q = -gridRadius; q <= gridRadius; q++)
        {
            int r1 = Mathf.Max(-gridRadius, -q - gridRadius);
            int r2 = Mathf.Min(gridRadius, -q + gridRadius);
            for (int r = r1; r <= r2; r++)
            {
                Vector2Int hexCoord = new Vector2Int(q, r);
                Hex hex = new Hex(hexCoord);
                hexes[hexCoord] = hex;
            }
        }
    }
    public void ScaleGrid()
    {
        float scaleFactor = 1.5f; // Fixed scale factor for uniform scaling

        hexSize *= scaleFactor;

        // Trigger the event with the fixed scale factor
        OnGridScaled.Invoke(scaleFactor);
    }

    // Marks the center cell as occupied to prevent it from being used for attachments
    void MarkCenterCellAsOccupied()
    {
        Vector2Int centerCell = new Vector2Int(0, 0);
        occupiedCells.Add(centerCell); // Mark the center as occupied
    }

    // Converts axial hex coordinates to world space
    public Vector3 HexToWorldPosition(Vector2Int hexCoord)
    {
        float x = hexSize * (3f / 2f * hexCoord.x);
        float y = hexSize * (Mathf.Sqrt(3) * (hexCoord.y + hexCoord.x / 2f));
        return new Vector3(x, y, 0);
    }
    public List<Vector2Int> GetHexRing(int ringRadius)
    {
        List<Vector2Int> ringCells = new List<Vector2Int>();

        if (ringRadius == 0)
        {
            // Return just the center cell
            ringCells.Add(new Vector2Int(0, 0));
            return ringCells;
        }

        Vector2Int start = new Vector2Int(-ringRadius, 0);
        Vector2Int current = start;

        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < ringRadius; j++)
            {
                ringCells.Add(current);
                current = GetNextHexInDirection(current, i); // Move in the current direction
            }
        }

        return ringCells;
    }

    private Vector2Int GetNextHexInDirection(Vector2Int currentHex, int direction)
    {
        List<Vector2Int> directions = new List<Vector2Int>
    {
        new Vector2Int(1, 0), // East
        new Vector2Int(1, -1), // North-East
        new Vector2Int(0, -1), // North-West
        new Vector2Int(-1, 0), // West
        new Vector2Int(-1, 1), // South-West
        new Vector2Int(0, 1) // South-East
    };

        return currentHex + directions[direction];
    }

    public bool IsRingFullyOccupied(int ringRadius)
    {
        List<Vector2Int> ringCells = GetHexRing(ringRadius);

        foreach (Vector2Int cell in ringCells)
        {
            if (!IsCellOccupied(cell))
            {
                return false; // If any cell in the ring is not occupied, return false
            }
        }

        return true;
    }


    // Converts world position to axial hex coordinates
    public Vector2Int WorldToHexCoordinates(Vector3 worldPosition)
    {
        Vector3 relativePosition = worldPosition - ship.transform.position;

        float q = (2f / 3f * relativePosition.x) / hexSize;
        float r = (-1f / 3f * relativePosition.x + Mathf.Sqrt(3f) / 3f * relativePosition.y) / hexSize;

        return RoundToHexCoordinates(q, r);
    }

    // Rounds fractional axial coordinates to the nearest hex
    Vector2Int RoundToHexCoordinates(float q, float r)
    {
        int roundedQ = Mathf.RoundToInt(q);
        int roundedR = Mathf.RoundToInt(r);
        int roundedS = Mathf.RoundToInt(-q - r);

        float qDiff = Mathf.Abs(roundedQ - q);
        float rDiff = Mathf.Abs(roundedR - r);
        float sDiff = Mathf.Abs(roundedS + q + r);

        if (qDiff > rDiff && qDiff > sDiff)
        {
            roundedQ = -roundedR - roundedS;
        }
        else if (rDiff > sDiff)
        {
            roundedR = -roundedQ - roundedS;
        }

        return new Vector2Int(roundedQ, roundedR);
    }

    // Returns a list of the axial coordinates of all hexes
    public List<Vector2Int> GetAllHexCoordinates()
    {
        return new List<Vector2Int>(hexes.Keys);
    }

    // Track a cell as occupied when a new part is attached
    public void OccupyCell(Vector2Int cell)
    {
        reservedCells.Remove(cell); // Clear any reservation when a cell is fully occupied
        occupiedCells.Add(cell);
    }

    // Check if a cell is occupied or reserved
    public bool IsCellOccupied(Vector2Int cell)
    {
        return occupiedCells.Contains(cell) || reservedCells.Contains(cell);
    }

    // Reserve a cell to prevent multiple attachments from claiming it
    public bool ReserveCell(Vector2Int cell)
    {
        if (!IsCellOccupied(cell))
        {
            reservedCells.Add(cell);
            return true;
        }
        return false;
    }

    // Clear a reservation if the attachment doesn't reach the reserved cell
    public void ClearReservation(Vector2Int cell)
    {
        reservedCells.Remove(cell);
    }

    // Returns a list of the axial coordinates of neighboring hexes
    public List<Vector2Int> GetHexNeighbors(Vector2Int hex)
    {
        List<Vector2Int> directions = new List<Vector2Int>
        {
            new Vector2Int(1, 0),
            new Vector2Int(1, -1),
            new Vector2Int(0, -1),
            new Vector2Int(-1, 0),
            new Vector2Int(-1, 1),
            new Vector2Int(0, 1)
        };

        List<Vector2Int> neighbors = new List<Vector2Int>();

        foreach (var direction in directions)
        {
            neighbors.Add(hex + direction);
        }

        return neighbors;
    }

    void OnDrawGizmos()
    {
        if (hexes == null || hexes.Count == 0) return;

        Vector3 shipPosition = ship != null ? ship.transform.position : Vector3.zero;

        foreach (Hex hex in hexes.Values)
        {
            Vector3 worldPosition = HexToWorldPosition(hex.coordinates) + shipPosition;
            DrawHex(worldPosition);
            DrawHexTriangles(worldPosition, hex.coordinates);
        }
    }

    // Draws a single hexagon using Gizmos at the given world position
    void DrawHex(Vector3 center)
    {
        float radius = hexSize;
        Vector3[] corners = new Vector3[6];

        for (int i = 0; i < 6; i++)
        {
            float angle_deg = 60 * i;
            float angle_rad = Mathf.Deg2Rad * angle_deg;
            corners[i] = new Vector3(
                center.x + radius * Mathf.Cos(angle_rad),
                center.y + radius * Mathf.Sin(angle_rad),
                0
            );
        }

        for (int i = 0; i < 6; i++)
        {
            Gizmos.DrawLine(corners[i], corners[(i + 1) % 6]);
        }
    }

    // Draws the connections between neighboring hexes (dual graph edges)
    void DrawHexTriangles(Vector3 center, Vector2Int hexCoord)
    {
        List<Vector2Int> neighbors = GetHexNeighbors(hexCoord);

        foreach (Vector2Int neighbor in neighbors)
        {
            if (hexes.ContainsKey(neighbor))
            {
                Vector3 neighborCenter = HexToWorldPosition(neighbor) + (ship != null ? ship.transform.position : Vector3.zero);
                Gizmos.DrawLine(center, neighborCenter);
            }
        }
    }

    // Represents a single hex cell
    public class Hex
    {
        public Vector2Int coordinates;

        public Hex(Vector2Int coordinates)
        {
            this.coordinates = coordinates;
        }
    }
}