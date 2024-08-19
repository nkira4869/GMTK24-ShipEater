using UnityEngine;

public class LaserBeam : MonoBehaviour
{
    public float damagePerSecond = 10f; // Damage dealt by the laser per second
    public float maxLength = 10f; // Maximum length of the laser
    public LayerMask targetLayers; // Layers the laser can hit
    public LineRenderer lineRenderer; // LineRenderer component for the laser

    public float firingDuration = 1f; // How long the laser fires for in seconds
    public float cooldownDuration = 2f; // How long the laser is on cooldown in seconds

    private float firingTimer;
    private float cooldownTimer;
    private bool isFiring;

    void Start()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }
        firingTimer = firingDuration;
        cooldownTimer = 0f;
    }

    void Update()
    {
        if (isFiring)
        {
            FireLaser();
            firingTimer -= Time.deltaTime;

            if (firingTimer <= 0f)
            {
                isFiring = false;
                lineRenderer.enabled = false;
                cooldownTimer = cooldownDuration;
            }
        }
        else
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0f)
            {
                StartFiring();
            }
        }
    }

    void StartFiring()
    {
        isFiring = true;
        firingTimer = firingDuration;
        lineRenderer.enabled = true;
    }

    void FireLaser()
    {
        // Use RaycastAll to detect all targets along the laser's path
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, transform.up, maxLength, targetLayers);
        
        if (hits.Length > 0)
        {
            // Set the laser's start and end points
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, hits[0].point);

            // Iterate through all hit targets and apply damage
            foreach (RaycastHit2D hit in hits)
            {
                Health health = hit.collider.GetComponent<Health>();
                if (health != null)
                {
                    health.TakeDamage(damagePerSecond * Time.deltaTime);
                }
            }
        }
        else
        {
            // No targets hit, so extend the laser to its full length
            lineRenderer.SetPosition(0, transform.position);
            lineRenderer.SetPosition(1, transform.position + transform.up * maxLength);
        }
    }
}