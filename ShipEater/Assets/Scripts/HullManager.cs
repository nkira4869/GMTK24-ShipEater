using System.Collections.Generic;
using UnityEngine;

public class HullManager : MonoBehaviour
{
    public Health hullHealth; // Reference to the existing Health component
    public ShipHexGridSystem hexGridSystem;
    public GameObject defaultBulletPrefab; // Default bullet prefab for any activated bullet patterns
    public PlayerController playerController; // Reference to the PlayerController
    private List<Attachment> attachments = new List<Attachment>();

    [Header("Leveling System")]
    public int currentLevel = 1;
    public int levelUpThreshold = 5;
    public float hexRadiusExpansionAmount = 0.5f;
    public int immuneAttachmentsCount = 2;

    void Start()
    {
        if (hullHealth == null)
        {
            hullHealth = gameObject.GetComponent<Health>();
        }

        // Ensure the PlayerController is assigned
        if (playerController == null)
        {
            playerController = GetComponent<PlayerController>();
        }

        // Subscribe to health events
        hullHealth.onHealthChanged += OnHealthChanged;
        hullHealth.onDeath += OnHullDestroyed;
    }

    public void AddAttachment(Attachment newAttachment)
    {
        attachments.Add(newAttachment);
        newAttachment.Attach(hexGridSystem);

        if (attachments.Count % levelUpThreshold == 0)
        {
            LevelUp();
        }
    }

    // Method to apply a health modifier 
    public void ApplyHealthModifier(float modifier)
    {
        hullHealth.maxHealth += modifier;
        hullHealth.currentHealth = Mathf.Clamp(hullHealth.currentHealth, 0, hullHealth.maxHealth);
    }

    // Method to apply a speed modifier to both horizontal and vertical speeds
    public void ApplySpeedModifier(float modifier)
    {
        if (playerController != null)
        {
            playerController.ApplyHorizontalSpeedModifier(modifier);
            playerController.ApplyVerticalSpeedModifier(modifier);
        }
    }

    void OnHealthChanged(float currentHealth, float maxHealth)
    {
        Debug.Log($"Hull Health Changed: {currentHealth}/{maxHealth}");
    }

    void LevelUp()
    {
        currentLevel++;
        List<Attachment> closestAttachments = FindClosestAttachmentsToCenter(attachments, immuneAttachmentsCount);
        foreach (var attachment in closestAttachments)
        {
            attachment.MakeImmuneToDamage();
        }

        ExpandGridRadius();
        Debug.Log($"Hull leveled up to level {currentLevel}. {immuneAttachmentsCount} attachments are now immune to damage.");
    }

    List<Attachment> FindClosestAttachmentsToCenter(List<Attachment> allAttachments, int count)
    {
        allAttachments.Sort((a, b) => Vector2Int.Distance(Vector2Int.zero, a.gridPosition).CompareTo(Vector2Int.Distance(Vector2Int.zero, b.gridPosition)));
        return allAttachments.GetRange(0, Mathf.Min(count, allAttachments.Count));
    }

    void ExpandGridRadius()
    {
        hexGridSystem.IncreaseHexRadius(hexRadiusExpansionAmount);
        foreach (var attachment in attachments)
        {
            Vector3 newPosition = hexGridSystem.GetWorldPosition(attachment.gridPosition);
            attachment.transform.position = newPosition;
        }
    }

    void OnHullDestroyed()
    {
        Debug.Log("Hull Destroyed!");
        Destroy(gameObject);
    }
}