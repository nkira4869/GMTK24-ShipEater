using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f; // Maximum health
    public float currentHealth;
    private bool isImmune = false; // Track whether the object is immune to damage

    public delegate void OnHealthChanged(float currentHealth, float maxHealth);
    public event OnHealthChanged onHealthChanged; // Event to notify when health changes

    public delegate void OnDeath();
    public event OnDeath onDeath; // Event to notify when the object dies

    void Start()
    {
        currentHealth = maxHealth; // Initialize current health
    }

    // Method to apply damage
    public void TakeDamage(float damage)
    {
        if (isImmune) return; // Do nothing if the object is immune to damage

        currentHealth -= damage;
        onHealthChanged?.Invoke(currentHealth, maxHealth); // Notify listeners about the health change

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Method to set immunity status
    public void SetImmune(bool immuneStatus)
    {
        isImmune = immuneStatus;
    }

    // Method to heal or restore health
    public void Heal(float healAmount)
    {
        currentHealth += healAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Ensure health doesn't exceed maxHealth
        onHealthChanged?.Invoke(currentHealth, maxHealth); // Notify listeners about the health change
    }

    // Method called when health reaches zero
    void Die()
    {
        onDeath?.Invoke(); // Notify listeners about the object's death
        Destroy(gameObject); // Destroy the object (or trigger some death logic)
    }
}