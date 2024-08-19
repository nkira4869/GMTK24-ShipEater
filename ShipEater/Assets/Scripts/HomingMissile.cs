using UnityEngine;

public class HomingMissile : MonoBehaviour
{

    public float speed = 5f;
    public float damage = 15f;
    public float rotationSpeed = 200f;
    public LayerMask targetLayers;
    public float lifetime = 5f;
    public float explosionRadius = 3f;
    
    public GameObject explosionEffect;

    private Transform target;

    void Start()
    {
        // Find the nearest target
        FindTarget();
        Destroy(gameObject, lifetime); // Destroy the bullet after its lifetime expires
    }

    void Update()
    {
        if (target != null)
        {
            // Calculate the direction to the target
            Vector2 direction = (Vector2)target.position - (Vector2)transform.position;
            direction.Normalize();

            // Rotate towards the target
            float rotateAmount = Vector3.Cross(direction, transform.up).z;
            transform.Rotate(0, 0, -rotateAmount * rotationSpeed * Time.deltaTime);

            // Move the bullet forward
            transform.Translate(Vector3.up * speed * Time.deltaTime);
        }
        else
        {
            // If no target is found, move straight
            transform.Translate(Vector3.up * speed * Time.deltaTime);
        }
    }

    void FindTarget()
    {
        // Find the closest target on the target layers
        Collider2D[] possibleTargets = Physics2D.OverlapCircleAll(transform.position, 10f, targetLayers);
        float closestDistance = Mathf.Infinity;

        foreach (Collider2D potentialTarget in possibleTargets)
        {
            float distanceToTarget = Vector2.Distance(transform.position, potentialTarget.transform.position);
            if (distanceToTarget < closestDistance)
            {
                closestDistance = distanceToTarget;
                target = potentialTarget.transform;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        
        if (((1 << other.gameObject.layer) & targetLayers) != 0)
        {
            Explode();
        }
    }

    void Explode()
    {
        // Create an explosion effect (optional)
        if (explosionEffect != null)
        {
            GameObject obj = Instantiate(explosionEffect, transform.position, Quaternion.identity);
            Destroy(obj, 0.5f);
        }

        // Deal damage to all objects within the explosion radius
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius, targetLayers);
        foreach (Collider2D hit in hits)
        {
            Health health = hit.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
            }
        }

        // Destroy the rocket after the explosion
        Destroy(gameObject);
    }

    void OnDrawGizmos()
    {
        // Visualize the explosion radius in the editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}
