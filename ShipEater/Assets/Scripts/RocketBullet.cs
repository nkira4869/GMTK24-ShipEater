using UnityEngine;

public class RocketBullet : MonoBehaviour
{
    public float speed = 5f;
    public float damage = 20f;
    public float explosionRadius = 3f;
    public LayerMask targetLayers;
    public GameObject explosionEffect;

    void Update()
    {
        // Move the rocket forward
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the rocket hits something on the target layers
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