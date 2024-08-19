using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Enemy Configuration Variables
    public Sprite enemySprite;
    public float bulletDamage = 10f;
    public float health = 100f;
    public float movementSpeed = 2f;
    public GameObject debrisPrefab;
    public bool canDropDebris = false;
    public float debrisDropChancePercent = 50f; // Probability of debris dropping (0.5 = 50% chance)
    public Animator animator;
    public float lifespan = 30f;

    // Strafing Variables
    public bool canStrafe = true;
    public float strafeAmount = 0.5f;
    public float strafeSpeed = 2f;
    public float strafePauseMinDuration = 1f;
    public float strafePauseMaxDuration = 3f;
    public float strafeResumeMinDelay = 5f;
    public float strafeResumeMaxDelay = 10f;

    private Health healthComponent;
    private bool isStrafing = true;
    private float randomStrafeOffset;
    private float strafeDirection;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    void Start()
    {
        // Setup the enemy sprite
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = enemySprite;
        originalColor = spriteRenderer.color;

        // Add and initialize the health component
        healthComponent = gameObject.GetComponent<Health>();
        healthComponent.maxHealth = health;
        healthComponent.currentHealth = health;

        // Assign the bullet damage to any existing bullet patterns (if manually added)
        var shooters = GetComponentsInChildren<Shooter>();
        foreach (var shooter in shooters)
        {
            if (TryGetComponent<Bullet>(out Bullet bl))
            {
                bl.damage = bulletDamage;
            }
            if (TryGetComponent<RocketBullet>(out RocketBullet rb))
            {
                rb.damage = bulletDamage;
            }
            if (TryGetComponent<HomingBullet>(out HomingBullet hb))
            {
                hb.damage = bulletDamage;
            }
            if (TryGetComponent<HomingMissile>(out HomingMissile hm))
            {
                hm.damage = bulletDamage;
            }

        }

        // Subscribe to the health component's events
        healthComponent.onHealthChanged += OnHit;
        healthComponent.onDeath += OnDeath;

        StartCoroutine(lifespanTrigger());
        StartCoroutine(ManageStrafing());

        randomStrafeOffset = Random.Range(0f, Mathf.PI * 2f);
        strafeDirection = Random.value > 0.5f ? 1f : -1f;
    }

    void Update()
    {
        MoveEnemy();
    }

    void MoveEnemy()
    {
        transform.Translate(Vector3.up * movementSpeed * Time.deltaTime);

        if (canStrafe && isStrafing)
        {
            Strafe();
        }
    }

    void Strafe()
    {
        float strafeMovement = Mathf.Sin(Time.time * strafeSpeed + randomStrafeOffset) * strafeAmount * strafeDirection;
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
            yield return new WaitForSeconds(Random.Range(strafeResumeMinDelay, strafeResumeMaxDelay));
            isStrafing = false;
            yield return new WaitForSeconds(Random.Range(strafePauseMinDuration, strafePauseMaxDuration));
            isStrafing = true;
        }
    }

    // Method to handle hit animation with a red flash
    void OnHit(float currentHealth, float maxHealth)
    {
        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }

        // Trigger the red flash
        StartCoroutine(FlashRed());
    }

    IEnumerator FlashRed()
    {
        // Change color to red
        spriteRenderer.color = Color.red;

        // Wait for a short duration
        yield return new WaitForSeconds(0.1f);

        // Revert back to the original color
        spriteRenderer.color = originalColor;
    }

    void OnDeath()
    {
        if (animator != null)
        {
            animator.SetTrigger("Death");
        }

        // Check if debris can drop and roll for the drop chance
        if (canDropDebris && debrisPrefab != null)
        {
            if (Random.value <= debrisDropChancePercent/100) // Random.value returns a float between 0 and 1
            {
                Instantiate(debrisPrefab, transform.position, Quaternion.identity);
            }
        }

        Destroy(gameObject, 1f);
    }
}