using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class ReadOnlyAttribute : PropertyAttribute { }

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUI.enabled = false; // Disable the field
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true; // Enable it back
    }
}
#endif

public class Health : MonoBehaviour
{
    public float maxHealth = 100f; // Maximum health
    [ReadOnly] private float currentHealth;

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
        currentHealth -= damage;
        onHealthChanged?.Invoke(currentHealth, maxHealth); // Notify listeners about the health change

        if (currentHealth <= 0)
        {
            Die();
        }
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