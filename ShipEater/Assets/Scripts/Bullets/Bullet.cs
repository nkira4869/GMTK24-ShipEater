using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 5f; // Bullet speed
    public float damage = 10f; // Damage dealt by the bullet
    public float lifetime = 5f; // Time before the bullet is destroyed
    public LayerMask targetLayers; // Layers that this bullet can hit

    void Start()
    {
        Destroy(gameObject, lifetime); // Destroy the bullet after its lifetime expires
    }

    void Update()
    {
        // Move the bullet forward
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collided object is on the target layers
        if (((1 << other.gameObject.layer) & targetLayers) != 0)
        {
            Debug.Log("Hit" + other.name);
            // Apply damage if the object has a Health component
            Health health = other.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage);
                Destroy(gameObject); // Destroy the bullet after hitting an object
            }
        }
    }
}