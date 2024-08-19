using System.Collections;
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
    public float lifespan = 30f;

    // Strafing Variables
    public bool canStrafe = true; // Toggle strafing behavior
    public float strafeAmount = 0.5f; // How far to strafe left and right
    public float strafeSpeed = 2f; // Speed of the strafing motion
    public float strafePauseMinDuration = 1f; // Minimum duration of the pause
    public float strafePauseMaxDuration = 3f; // Maximum duration of the pause
    public float strafeResumeMinDelay = 5f; // Minimum time between pauses
    public float strafeResumeMaxDelay = 10f; // Maximum time between pauses

    private Health healthComponent;
    private bool isStrafing = true; // Track whether the enemy is currently strafing
    private float randomStrafeOffset; // Random offset to desynchronize strafing patterns
    private float strafeDirection; // The direction the enemy strafes in (1 for right, -1 for left)

    void Start()
    {
        // Setup the enemy sprite
        GetComponent<SpriteRenderer>().sprite = enemySprite;

        // Add and initialize the health component
        healthComponent = gameObject.GetComponent<Health>();
        healthComponent.maxHealth = health;
        healthComponent.currentHealth = health;

        // Assign the bullet damage to any existing bullet patterns (if manually added)
        var shooters = GetComponentsInChildren<Shooter>();
        foreach (var shooter in shooters)
        {
            shooter.bulletPrefab.GetComponent<Bullet>().damage = bulletDamage;
        }

        // Subscribe to the health component's events
        healthComponent.onHealthChanged += OnHit;
        healthComponent.onDeath += OnDeath;

        StartCoroutine(lifespanTrigger());
        StartCoroutine(ManageStrafing()); // Start strafing behavior

        // Set a random offset to desynchronize strafing patterns
        randomStrafeOffset = Random.Range(0f, Mathf.PI * 2f);

        // Randomize the initial strafe direction
        strafeDirection = Random.value > 0.5f ? 1f : -1f;
    }

    void Update()
    {
        // Handle enemy movement
        MoveEnemy();
    }

    void MoveEnemy()
    {
        // Move forward (simple downward movement for this example)
        transform.Translate(Vector3.up * movementSpeed * Time.deltaTime);

        // Apply strafing motion if enabled and strafing is active
        if (canStrafe && isStrafing)
        {
            Strafe();
        }
    }

    void Strafe()
    {
        // Calculate strafe position with random offset
        float strafeMovement = Mathf.Sin(Time.time * strafeSpeed + randomStrafeOffset) * strafeAmount * strafeDirection;

        // Apply the strafe movement (side-to-side)
        transform.Translate(Vector3.right * strafeMovement * Time.deltaTime);
    }

    IEnumerator lifespanTrigger()
    {
        yield return new WaitForSeconds(lifespan);
        healthComponent.Die();
    }

    IEnumerator ManageStrafing()
    {
        while (true)
        {
            // Wait for a random duration before pausing strafing
            yield return new WaitForSeconds(Random.Range(strafeResumeMinDelay, strafeResumeMaxDelay));

            // Pause strafing
            isStrafing = false;

            // Wait for the pause duration
            yield return new WaitForSeconds(Random.Range(strafePauseMinDuration, strafePauseMaxDuration));

            // Resume strafing
            isStrafing = true;
        }
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