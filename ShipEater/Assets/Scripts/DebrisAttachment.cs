using UnityEngine;

public class DebrisAttachment : MonoBehaviour
{
    public float pullSpeed = 2f; // The speed at which debris is pulled towards the ship
    public float minDetectionRange = 2f; // Minimum detection range
    public float maxDetectionRange = 5f; // Maximum detection range
    public LineRenderer lineRenderer; // LineRenderer for visualizing the pull

    private ShipHexGridSystem shipGridSystem;
    private Transform shipTransform;
    private Vector2Int targetGridPosition;
    private bool isBeingPulled = false;
    private bool isAttached = false;
    private float detectionRange; // Randomized detection range for this debris
    private CircleCollider2D circleCollider; // Reference to the CircleCollider2D

    void Start()
    {
        // Set a random detection range within the defined min and max values
        detectionRange = Random.Range(minDetectionRange, maxDetectionRange);

        // Adjust the CircleCollider2D radius to match the detection range
        circleCollider = GetComponent<CircleCollider2D>();
        if (circleCollider != null)
        {
            circleCollider.radius = detectionRange;
        }

        // Get the reference to the ship's grid system
        shipGridSystem = FindObjectOfType<ShipHexGridSystem>();

        // Initialize the LineRenderer if not already assigned
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = Color.yellow;
            lineRenderer.endColor = Color.yellow;
            lineRenderer.enabled = false; // Start with the LineRenderer disabled
        }
    }

    void Update()
    {
        if (isBeingPulled && !isAttached)
        {
            Vector3 targetWorldPosition = shipGridSystem.GetWorldPosition(targetGridPosition); // Get world position of the target grid cell
            transform.position = Vector3.MoveTowards(transform.position, targetWorldPosition, pullSpeed * Time.deltaTime);

            // Update the LineRenderer positions
            lineRenderer.SetPosition(0, transform.position); // Start point at the debris position
            lineRenderer.SetPosition(1, shipTransform.position); // End point at the ship's position

            // Check if the debris has reached the target grid cell
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

        // Change the debris layer to "Player" (make sure you have a layer named "Player")
        gameObject.layer = LayerMask.NameToLayer("Player");
        gameObject.tag = "Player";

        // Mark the grid cell as occupied and expand the grid
        shipGridSystem.MarkCellAsOccupied(targetGridPosition);

        // Remove the cell from the reserved list
        shipGridSystem.RemoveReservedCell(targetGridPosition);

        circleCollider.radius = 0.2f;

        // Attach the debris to the ship
        transform.SetParent(shipTransform); // Make the debris a child of the ship
        lineRenderer.enabled = false; // Disable the LineRenderer after attaching

        // Stop the debris movement when attached
        GetComponent<DebrisMovement>().AttachToShip();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isBeingPulled && !isAttached)
        {
            shipTransform = other.transform;
            targetGridPosition = shipGridSystem.FindNearestEmptyCell(transform.position);
            isBeingPulled = true;

            // Enable the LineRenderer when pulling starts
            lineRenderer.enabled = true;
        }
    }

    // Optional: Visualize the detection range in the editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}