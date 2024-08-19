using UnityEngine;

public class DebrisAttachment : MonoBehaviour
{
    public float pullSpeed = 2f; // The speed at which debris is pulled towards the ship
    public float minDetectionRange = 2f; // Minimum detection range
    public float maxDetectionRange = 5f; // Maximum detection range
    public LineRenderer lineRenderer; // LineRenderer for visualizing the pull

    // New Variables
    public float healthModifier = 1.1f; // Health modifier (multiplicative)
    public float speedModifier = 0.5f; // Speed modifier (additive)
    public GameObject bulletPatternPrefab; // Reference to a bullet pattern that will be activated on attachment

    private ShipHexGridSystem shipGridSystem;
    private HullManager hullManager;
    private Transform shipTransform;
    private Vector2Int targetGridPosition;
    private bool isBeingPulled = false;
    private bool isAttached = false;
    private float detectionRange; // Randomized detection range for this debris
    private CircleCollider2D circleCollider;

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
        hullManager = FindObjectOfType<HullManager>();

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
            Vector3 targetWorldPosition = shipGridSystem.GetWorldPosition(targetGridPosition);
            transform.position = Vector3.MoveTowards(transform.position, targetWorldPosition, pullSpeed * Time.deltaTime);

            // Update the LineRenderer positions
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, shipTransform.position);

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

        // Change the debris layer to "Player"
        gameObject.layer = LayerMask.NameToLayer("Player");
        gameObject.tag = "Player";

        // Mark the grid cell as occupied and expand the grid
        shipGridSystem.MarkCellAsOccupied(targetGridPosition);
        shipGridSystem.RemoveReservedCell(targetGridPosition);

        circleCollider.radius = 0.2f;

        // Attach the debris to the ship
        transform.SetParent(shipTransform);
        lineRenderer.enabled = false;

        // Stop the debris movement when attached
        GetComponent<DebrisMovement>().AttachToShip();

        // Dynamically add the Attachment script and register it with the HullManager
        if (hullManager != null)
        {
            Attachment attachment = gameObject.AddComponent<Attachment>();
            attachment.gridPosition = targetGridPosition;
            hullManager.AddAttachment(attachment);

            // Apply health and speed modifiers
            hullManager.ApplyHealthModifier(healthModifier);
            hullManager.ApplySpeedModifier(speedModifier);
        }

        // Activate the bullet pattern if one is assigned
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