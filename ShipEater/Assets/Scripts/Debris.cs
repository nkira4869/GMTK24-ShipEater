using UnityEngine;

public class Debris : MonoBehaviour
{
    public float detectionRadius = 5f; // The detection radius for the ship
    public float pullSpeed = 2f; // The speed at which the debris is pulled towards the ship

    private Transform shipTransform;
    private bool isBeingPulled = false;
    private LineRenderer lineRenderer;

    void Start()
    {
        // Initialize and set up the Line Renderer
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;

        // Set the default material and color
        lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // Using a basic sprite shader
        lineRenderer.startColor = Color.yellow;
        lineRenderer.endColor = Color.yellow;

        // Set other line renderer properties as needed
        lineRenderer.positionCount = 2; // Two points: start and end
        lineRenderer.enabled = false; // Start with it disabled until we need it
    }

    void Update()
    {
        // Detect the player ship within the detection radius
        Collider2D playerCollider = Physics2D.OverlapCircle(transform.position, detectionRadius, LayerMask.GetMask("Player"));

        if (playerCollider != null && !isBeingPulled)
        {
            // Start pulling if the player is detected and we're not already pulling
            shipTransform = playerCollider.transform;
            isBeingPulled = true;
        }

        if (isBeingPulled && shipTransform != null)
        {
            // Enable and update the line renderer
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, transform.position); // Start point at the debris position
            lineRenderer.SetPosition(1, shipTransform.position); // End point at the ship's position

            // Pull the debris towards the ship
            transform.position = Vector3.MoveTowards(transform.position, shipTransform.position, pullSpeed * Time.deltaTime);

            // If the debris is close enough to the ship, "attach" it
            if (Vector3.Distance(transform.position, shipTransform.position) < 0.5f)
            {
                AttachToShip();
            }
        }
    }

    void AttachToShip()
    {
        // Attach the debris to the ship and stop pulling
        transform.SetParent(shipTransform);
        GetComponent<Rigidbody2D>().isKinematic = true;
        lineRenderer.enabled = false; // Disable the line renderer after attaching
        isBeingPulled = false; // Stop the pulling process
    }

    // Optional: Visualize the detection radius in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}