using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HullManager : MonoBehaviour
{
    public Health hullHealth;
    public DynamicHexGrid hexGridSystem; // Reference to the DynamicHexGrid
    public GameObject defaultBulletPrefab;
    public PlayerController playerController;
    private List<Attachment> attachments = new List<Attachment>();
    private Camera mainCamera;

    [Header("Leveling System")]
    public List<LevelConfig> levelConfigs; // List of level configurations
    private int currentLevel = 1;

    public float hexRadiusExpansionAmount = 0.5f;
    public float scaleUpAmount = 0.1f;
    private float previousHexSize;

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
        previousHexSize = hexGridSystem.hexSize;
        mainCamera = Camera.main;
        hullHealth.onHealthChanged += OnHealthChanged;
        hullHealth.onDeath += OnHullDestroyed;
    }

    public void AddAttachment(Attachment newAttachment)
    {
        attachments.Add(newAttachment);
        newAttachment.Attach(hexGridSystem);

        CheckForLevelChange();
    }

    public void RemoveAttachment(Attachment attachmentToRemove)
    {
        if (attachments.Contains(attachmentToRemove))
        {
            attachments.Remove(attachmentToRemove);
            CheckForLevelChange();
        }
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
        //Debug.Log($"Hull Health Changed: {currentHealth}/{maxHealth}");
    }

    void CheckForLevelChange()
    {
        if (currentLevel - 1 < levelConfigs.Count)
        {
            int currentLevelThreshold = levelConfigs[currentLevel - 1].levelUpThreshold;

            if (attachments.Count >= currentLevelThreshold && currentLevel < levelConfigs.Count)
            {
                LevelUp();
            }
            else if (currentLevel > 1 && attachments.Count < levelConfigs[currentLevel - 2].levelUpThreshold)
            {
                LevelDown();
            }
        }
    }

    void LevelUp()
    {
        currentLevel++;

        if (currentLevel - 1 < levelConfigs.Count)
        {
            LevelConfig config = levelConfigs[currentLevel - 1];
            List<Attachment> closestAttachments = FindClosestAttachmentsToCenter(attachments, config.immuneAttachmentsCount);

            foreach (var attachment in closestAttachments)
            {
                attachment.MakeImmuneToDamage();
            }

            ExpandGridRadius();
            NotifyAttachmentsOfGridScale();
            CameraController.Instance.UpdateCameraZoom(currentLevel, levelConfigs.Count); // Notify the camera controller to update the zoom
            Debug.Log($"Hull leveled up to level {currentLevel}. {config.immuneAttachmentsCount} attachments are now immune to damage.");
        }
    }

    void LevelDown()
    {
        currentLevel--;

        if (currentLevel - 1 < levelConfigs.Count && currentLevel > 0)
        {
            LevelConfig config = levelConfigs[currentLevel - 1];

            foreach (var attachment in attachments)
            {
                attachment.RemoveImmunity();
            }

            List<Attachment> closestAttachments = FindClosestAttachmentsToCenter(attachments, config.immuneAttachmentsCount);

            foreach (var attachment in closestAttachments)
            {
                attachment.MakeImmuneToDamage();
            }

            ShrinkGridRadius();
            NotifyAttachmentsOfGridScale();
            CameraController.Instance.UpdateCameraZoom(currentLevel, levelConfigs.Count); // Notify the camera controller to update the zoom
            Debug.Log($"Hull leveled down to level {currentLevel}. {config.immuneAttachmentsCount} attachments are now immune to damage.");
        }
    }

    void ExpandGridRadius()
    {
        // Expand the grid size (increase the hex size)
        hexGridSystem.hexSize += hexRadiusExpansionAmount;
    }

    void ShrinkGridRadius()
    {
        // Shrink the grid size (decrease the hex size)
        hexGridSystem.hexSize -= hexRadiusExpansionAmount;
    }

    void NotifyAttachmentsOfGridScale()
    {
        float scaleFactor = hexGridSystem.hexSize / previousHexSize; // Calculate the scale change factor relative to the previous size.
        previousHexSize = hexGridSystem.hexSize; // Update the previous size to the current one.

        foreach (var attachment in attachments)
        {
            attachment.OnGridScaled(scaleFactor); // Pass the scale factor to each attachment.
        }

        // Adjust the hull sprite scaling separately, multiplying by the scale factor
        Transform hullSpriteTransform = transform.Find("HullSprite");
        if (hullSpriteTransform != null)
        {
            hullSpriteTransform.localScale *= scaleFactor;
        }
    }

    List<Attachment> FindClosestAttachmentsToCenter(List<Attachment> allAttachments, int count)
    {
        allAttachments.Sort((a, b) => Vector2Int.Distance(Vector2Int.zero, a.gridPosition).CompareTo(Vector2Int.Distance(Vector2Int.zero, b.gridPosition)));
        return allAttachments.GetRange(0, Mathf.Min(count, allAttachments.Count));
    }

    void OnHullDestroyed()
    {
        Debug.Log("Hull Destroyed!");
        Destroy(gameObject);
    }
}