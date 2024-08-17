using UnityEngine;

public class Debris : MonoBehaviour
{
    public float detectionRadius = 5f; // The detection radius for the ship
    public float pullSpeed = 2f; // The speed at which the debris is pulled towards the ship

    private Transform shipTransform;
    private bool isBeingPulled = false;
    private LineRenderer lineRenderer;
    private Vector3 attachmentPoint; // The point where the debris will attach to the ship
    private bool isAttached = false;

    void Start()
    {
        // Initialize and set up the Line Renderer
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // Using a basic sprite shader
        lineRenderer.startColor = Color.yellow;
        lineRenderer.endColor = Color.yellow;
        lineRenderer.enabled = false; // Start with it disabled until we need it
    }

    void Update()
    {
        // Detect the player ship within the detection radius
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, detectionRadius, LayerMask.GetMask("Player"));

        if (playerCollider != null && !isBeingPulled && !isAttached)
        {
            // Start pulling if the player is detected and we're not already pulling or attached
            shipTransform = playerCollider.transform;
            isBeingPulled = true;
        }

        if (isBeingPulled && shipTransform != null && !isAttached)
        {
            // Enable and update the line renderer
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, transform.position); // Start point at the debris position
            lineRenderer.SetPosition(1, shipTransform.position); // End point at the ship's position

            // Pull the debris towards the ship
            transform.position = Vector3.MoveTowards(transform.position, shipTransform.position, pullSpeed * Time.deltaTime);

            // If the debris is close enough to the ship, set the contact point for attachment
            if (Vector3.Distance(transform.position, shipTransform.position) < 0.5f)
            {
                // Calculate the point of contact (this would be the ship's surface closest to the debris)
                attachmentPoint = transform.position - shipTransform.position;
                AttachToShip();
            }
        }

        // If the debris is attached, ensure it maintains its relative position
        if (isAttached)
        {
            transform.position = shipTransform.position + attachmentPoint;
        }
    }

    void AttachToShip()
    {
        // Stop pulling and fix the debris to the ship
        isBeingPulled = false;
        isAttached = true;

        // Disable the line renderer after attaching
        lineRenderer.enabled = false;

        // Set the debris to be a child of the ship so it moves with the ship
        transform.SetParent(shipTransform);
        GetComponent<Rigidbody2D>().isKinematic = true; // Make the debris kinematic after attaching to avoid unintended physics interactions
    }

    // Optional: Visualize the detection radius in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}