using System.Collections.Generic;
using UnityEngine;

public class HullManager : MonoBehaviour
{
    public Health hullHealth;
    public ShipHexGridSystem hexGridSystem;
    public GameObject defaultBulletPrefab;
    public PlayerController playerController;
    private List<Attachment> attachments = new List<Attachment>();

    [Header("Leveling System")]
    public List<LevelConfig> levelConfigs; // List of level configurations
    private int currentLevel = 1;

    public float hexRadiusExpansionAmount = 0.5f;
    public float scaleUpAmount = 0.1f;

    void Start()
    {
        if (hullHealth == null)
        {
            hullHealth = gameObject.GetComponent<Health>();
        }

        if (playerController == null)
        {
            playerController = GetComponent<PlayerController>();
        }

        hullHealth.onHealthChanged += OnHealthChanged;
        hullHealth.onDeath += OnHullDestroyed;
    }

    public void AddAttachment(Attachment newAttachment)
    {
        attachments.Add(newAttachment);
        newAttachment.Attach(hexGridSystem);

        CheckForLevelUp();
    }

    public void ApplyHealthModifier(float modifier)
    {
        hullHealth.maxHealth += modifier;
        hullHealth.currentHealth = Mathf.Clamp(hullHealth.currentHealth, 0, hullHealth.maxHealth);
    }

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

    void CheckForLevelUp()
    {
        // Make sure we have a level configuration for the current level
        if (currentLevel - 1 < levelConfigs.Count)
        {
            int nextLevelThreshold = levelConfigs[currentLevel - 1].levelUpThreshold;

            // If the current attachment count meets or exceeds the next level's threshold, level up
            if (attachments.Count >= nextLevelThreshold)
            {
                LevelUp();
            }
        }
    }

    void LevelUp()
    {
        // Increase the current level
        currentLevel++;

        if (currentLevel - 1 < levelConfigs.Count)
        {
            // Get the configuration for the new level
            LevelConfig config = levelConfigs[currentLevel - 1];

            // Make the specified number of attachments immune
            List<Attachment> closestAttachments = FindClosestAttachmentsToCenter(attachments, config.immuneAttachmentsCount);
            foreach (var attachment in closestAttachments)
            {
                attachment.MakeImmuneToDamage();
            }

            ExpandGridRadiusAndScaleHull();
            Debug.Log($"Hull leveled up to level {currentLevel}. {config.immuneAttachmentsCount} attachments are now immune to damage.");
        }
        else
        {
            Debug.Log("No more level configurations available.");
        }
    }

    List<Attachment> FindClosestAttachmentsToCenter(List<Attachment> allAttachments, int count)
    {
        allAttachments.Sort((a, b) => Vector2Int.Distance(Vector2Int.zero, a.gridPosition).CompareTo(Vector2Int.Distance(Vector2Int.zero, b.gridPosition)));
        return allAttachments.GetRange(0, Mathf.Min(count, allAttachments.Count));
    }

    void ExpandGridRadiusAndScaleHull()
    {
        hexGridSystem.IncreaseHexRadius(hexRadiusExpansionAmount);

        Vector3 scaleIncrease = new Vector3(scaleUpAmount, scaleUpAmount, 0);
        transform.localScale += scaleIncrease;

        foreach (var attachment in attachments)
        {
            attachment.transform.localScale = Vector3.one; // Reset the attachment scale to (1,1,1)
            Vector3 newPosition = hexGridSystem.GetWorldPosition(attachment.gridPosition);
            attachment.transform.position = newPosition; // Reposition attachment according to the expanded grid
        }
    }

    void OnHullDestroyed()
    {
        Debug.Log("Hull Destroyed!");
        Destroy(gameObject);
    }
}