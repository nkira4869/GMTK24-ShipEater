using System.Collections.Generic;
using UnityEngine;

public class DebrisAttachment : MonoBehaviour
{
    public float pullSpeed = 2f; // The speed at which debris is pulled towards the ship
    public float minDetectionRange = 2f; // Minimum detection range
    public float maxDetectionRange = 5f; // Maximum detection range
    public LineRenderer lineRenderer; // LineRenderer for visualizing the pull

    public float healthModifier = 1.1f; // Health modifier (multiplicative)
    public float speedModifier = 0.5f; // Speed modifier (additive)
    public GameObject bulletPatternPrefab; // Reference to a bullet pattern that will be activated on attachment

    private DynamicHexGrid shipGridSystem;
    private HullManager hullManager;
    private Transform shipTransform;
    private Vector2Int targetGridPosition;
    private bool isBeingPulled = false;
    private bool isAttached = false;
    private float detectionRange; // Randomized detection range for this debris
    private CircleCollider2D circleCollider;

    void Start()
    {
        detectionRange = Random.Range(minDetectionRange, maxDetectionRange);

        circleCollider = GetComponent<CircleCollider2D>();
        if (circleCollider != null)
        {
            circleCollider.radius = detectionRange;
        }

        shipGridSystem = FindObjectOfType<DynamicHexGrid>();
        hullManager = FindObjectOfType<HullManager>();

        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = Color.yellow;
            lineRenderer.endColor = Color.yellow;
            lineRenderer.enabled = false;
        }
    }

    void Update()
    {
        if (isBeingPulled && !isAttached)
        {
            Vector3 targetWorldPosition = shipGridSystem.HexToWorldPosition(targetGridPosition) + shipTransform.position;
            transform.position = Vector3.MoveTowards(transform.position, targetWorldPosition, pullSpeed * Time.deltaTime);

            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, shipTransform.position);

            if (Vector3.Distance(transform.position, targetWorldPosition) < 0.1f)
            {
                AttachToShip();
            }
        }
    }

    void AttachToShip()
    {
        isBeingPulled = false;
        isAttached = true;

        gameObject.layer = LayerMask.NameToLayer("Player");
        gameObject.tag = "Player";

        shipGridSystem.OccupyCell(targetGridPosition);

        circleCollider.radius = 0.2f;

        transform.SetParent(shipTransform);
        transform.position = shipGridSystem.HexToWorldPosition(targetGridPosition) + shipTransform.position; // Align perfectly with the cell center
        lineRenderer.enabled = false;

        GetComponent<DebrisMovement>().AttachToShip();

        if (hullManager != null)
        {
            Attachment attachment = gameObject.AddComponent<Attachment>();
            attachment.gridPosition = targetGridPosition;
            hullManager.AddAttachment(attachment);

            hullManager.ApplyHealthModifier(healthModifier);
            hullManager.ApplySpeedModifier(speedModifier);
        }

        if (bulletPatternPrefab != null)
        {
            GameObject bulletPattern = Instantiate(bulletPatternPrefab, shipTransform);
            if (bulletPattern.TryGetComponent<Shooter>(out Shooter sh))
            {
                bulletPattern.GetComponent<Shooter>().shootPoint = this.gameObject.transform;
                bulletPattern.GetComponent<Shooter>().bulletPrefab = hullManager.defaultBulletPrefab;
            }

            bulletPattern.transform.SetParent(this.gameObject.transform);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isBeingPulled && !isAttached)
        {
            shipTransform = hullManager.transform;
            targetGridPosition = FindNearestUnoccupiedCellToCenter();

            // Attempt to reserve the cell; if it’s already reserved, find another cell
            if (!shipGridSystem.ReserveCell(targetGridPosition))
            {
                targetGridPosition = FindAnotherUnoccupiedCell();
            }

            isBeingPulled = true;
            lineRenderer.enabled = true;
        }
    }

    // Example of finding another unoccupied cell if the first choice is already reserved
    Vector2Int FindAnotherUnoccupiedCell()
    {
        List<Vector2Int> allCells = shipGridSystem.GetAllHexCoordinates();

        foreach (Vector2Int cell in allCells)
        {
            if (!shipGridSystem.IsCellOccupied(cell))
            {
                shipGridSystem.ReserveCell(cell);
                return cell;
            }
        }

        // Fallback: return center cell if no other options (should rarely happen)
        return new Vector2Int(0, 0);
    }

    // Find the nearest unoccupied cell to the center of the grid (relative to the ship)
    Vector2Int FindNearestUnoccupiedCellToCenter()
    {
        Vector2Int gridCenter = new Vector2Int(0, 0); // Center of the grid is usually (0, 0)
        List<Vector2Int> allCells = shipGridSystem.GetAllHexCoordinates(); // Assuming you have a method to get all grid cells

        allCells.Sort((a, b) =>
            Vector2Int.Distance(a, gridCenter).CompareTo(Vector2Int.Distance(b, gridCenter))
        );

        foreach (Vector2Int cell in allCells)
        {
            if (!shipGridSystem.IsCellOccupied(cell))
            {
                return cell;
            }
        }

        // Fallback: if all cells are occupied (which should rarely happen), return the center cell
        return gridCenter;
    }

    // Optional: Visualize the detection range in the editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
