using UnityEngine;

public class Debris : MonoBehaviour
{
    public float detectionRadius = 5f; // The detection radius for the ship
    public float pullSpeed = 2f; // The speed at which the debris is pulled towards the ship
    public LineRenderer lineRenderer; // The Line Renderer for the tractor beam

    private Transform shipTransform;
    private bool isBeingPulled = false;

    void Start()
    {
        // Ensure the Line Renderer is set up
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            lineRenderer.startColor = Color.yellow;
            lineRenderer.endColor = Color.yellow;
        }

        lineRenderer.enabled = false;
    }

    void Update()
    {
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Assuming the ship is tagged as "Player"
        {
            shipTransform = other.transform;
            isBeingPulled = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Stop pulling if the ship leaves the detection radius
            isBeingPulled = false;
            lineRenderer.enabled = false;
        }
    }

    // Optional: Visualize the detection radius in the editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}