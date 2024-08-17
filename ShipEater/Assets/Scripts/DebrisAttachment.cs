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

    void Start()
    {
        // Set a random detection range within the defined min and max values
        detectionRange = Random.Range(minDetectionRange, maxDetectionRange);

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
        shipGridSystem.OccupyCell(targetGridPosition); // Mark the grid cell as occupied
        transform.SetParent(shipTransform); // Make the debris a child of the ship
        lineRenderer.enabled = false; // Disable the LineRenderer after attaching
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isBeingPulled && !isAttached)
        {
            shipTransform = other.transform;

            // Check if the player ship is within the detection range
            if (Vector3.Distance(transform.position, shipTransform.position) <= detectionRange)
            {
                targetGridPosition = shipGridSystem.FindNearestEmptyCell(transform.position);
                isBeingPulled = true;

                // Enable the LineRenderer when pulling starts
                lineRenderer.enabled = true;
            }
        }
    }

    // Optional: Visualize the detection range in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}