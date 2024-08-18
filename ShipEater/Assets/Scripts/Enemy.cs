using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Enemy Configuration Variables
    public Sprite enemySprite; // The sprite to represent the enemy
    public float bulletDamage = 10f; // Bullet damage value
    public float health = 100f; // Enemy health
    public float movementSpeed = 2f; // Enemy movement speed
    public GameObject debrisPrefab; // Debris prefab to spawn on death
    public bool canDropDebris = false; // Boolean to check if the enemy can drop debris
    public Animator animator; // Reference to the Animator for hit and death animations

    private Health healthComponent;

    void Start()
    {
        // Setup the enemy sprite
        GetComponent<SpriteRenderer>().sprite = enemySprite;

        // Add and initialize the health component
        healthComponent = gameObject.GetComponent<Health>();
        healthComponent.maxHealth = health;
        healthComponent.currentHealth = health; // Ensure currentHealth is correctly initialized

        // Assign the bullet damage to any existing bullet patterns (if manually added)
        var shooters = GetComponentsInChildren<Shooter>();
        foreach (var shooter in shooters)
        {
            shooter.bulletPrefab.GetComponent<Bullet>().damage = bulletDamage;
        }

        // Subscribe to the health component's events
        healthComponent.onHealthChanged += OnHit;
        healthComponent.onDeath += OnDeath;
    }

    void Update()
    {
        // Handle enemy movement (simple downward movement for this example)
        transform.Translate(Vector3.up * movementSpeed * Time.deltaTime);
    }

    // Method to handle hit animation
    void OnHit(float currentHealth, float maxHealth)
    {
        // Trigger the hit animation
        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }
    }

    // Method to handle death animation and behavior
    void OnDeath()
    {
        // Trigger the death animation
        if (animator != null)
        {
            animator.SetTrigger("Death");
        }

        // Check if the enemy can drop debris
        if (canDropDebris && debrisPrefab != null)
        {
            // Spawn the debris prefab at the enemy's position
            Instantiate(debrisPrefab, transform.position, Quaternion.identity);
        }

        // Destroy the enemy after a delay (to let the death animation play)
        Destroy(gameObject, 1f); // Adjust the delay based on your death animation length
    }
}