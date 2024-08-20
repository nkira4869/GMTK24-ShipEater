using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

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
    public int currentLevel = 1;

    public float hexRadiusExpansionAmount = 0.5f;
    public float scaleUpAmount = 0.1f;
    private float previousHexSize;
    private int currentImmunityLevel = 0;
    private bool isScalingScheduled = false;

    [Header("Immunity Sprites")]
    public Sprite[] hullSprites; // Array of sprites corresponding to immunity levels 0 to 3
    private SpriteRenderer hullSpriteRenderer; // Reference to the sprite renderer for the hull

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

        hullSpriteRenderer = transform.Find("HullSprite").GetComponent<SpriteRenderer>();
        if (hullSpriteRenderer == null)
        {
            Debug.LogError("HullSprite object not found or missing SpriteRenderer component.");
        }
        UpdateHullSprite();
        StartCoroutine(RepositionAttachmentsPeriodically());
    }

    public void AddAttachment(Attachment newAttachment)
    {
        attachments.Add(newAttachment);
        newAttachment.Attach(hexGridSystem);
        CheckForLevelChange();
        CheckAndApplyRingImmunity();

    }
    private IEnumerator DelayedTriggerScalingEvent(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Trigger the scaling event
        ScaleAllComponents();

        // Reset the flag so the next scaling can be scheduled
        isScalingScheduled = false;
    }

    public void RemoveAttachment(Attachment attachmentToRemove)
    {
        if (attachments.Contains(attachmentToRemove))
        {
            attachments.Remove(attachmentToRemove);
            CheckForLevelChange();
            CheckAndApplyRingImmunity(); // Re-evaluate immunity after removal
        }
    }

    public void ApplyHealthModifier(float modifier)
    {
        hullHealth.maxHealth += modifier;
        hullHealth.currentHealth += modifier;
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

            // Update the camera zoom based on the current level
            CameraController.Instance.UpdateCameraZoom(currentLevel, levelConfigs.Count);

            Debug.Log($"Hull leveled up to level {currentLevel}.");

            // Delay the scaling and other attachment updates after the level up
            if (!isScalingScheduled)
            {
                isScalingScheduled = true;
                StartCoroutine(DelayedTriggerScalingEvent(1.0f)); // Adjust the delay time (in seconds) as needed
            }
        }
    }


    void LevelDown()
    {
        currentLevel--;

        if (currentLevel - 1 < levelConfigs.Count && currentLevel > 0)
        {
            LevelConfig config = levelConfigs[currentLevel - 1];

            // Update the camera zoom based on the current level
            CameraController.Instance.UpdateCameraZoom(currentLevel, levelConfigs.Count);

            Debug.Log($"Hull leveled down to level {currentLevel}.");
        }
    }

    private void ScaleAllComponents()
    {
        float scaleFactor = 1.5f; // Use the fixed scaling factor

        // Scale the grid
        hexGridSystem.ScaleGrid();

        // Scale the hull
        Transform hullSpriteTransform = transform.Find("HullSprite");
        if (hullSpriteTransform != null)
        {
            hullSpriteTransform.localScale *= scaleFactor;
        }
    }
    private IEnumerator RepositionAttachmentsPeriodically()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f); // Wait for 5 seconds
            RepositionAllAttachments();
        }
    }

    private void RepositionAllAttachments()
    {
        foreach (var attachment in attachments)
        {
            attachment.Reposition();
        }
    }

    List<Attachment> FindClosestAttachmentsToCenter(List<Attachment> allAttachments, int count)
    {
        allAttachments.Sort((a, b) => Vector2Int.Distance(Vector2Int.zero, a.gridPosition).CompareTo(Vector2Int.Distance(Vector2Int.zero, b.gridPosition)));
        return allAttachments.GetRange(0, Mathf.Min(count, allAttachments.Count));
    }
    private void CheckAndApplyRingImmunity()
    {
        int ringRadius = 1;
        int highestImmunityLevel = 0;

        while (ringRadius <= 3 && hexGridSystem.IsRingFullyOccupied(ringRadius))
        {
            highestImmunityLevel = ringRadius; // Set the immunity level based on the filled ring
            ringRadius++;
        }

        // Apply the new immunity level if it changed
        if (highestImmunityLevel != currentImmunityLevel)
        {
            currentImmunityLevel = highestImmunityLevel;
            UpdateHullSprite();
        }

        // Apply immunity to attachments in the occupied rings
        for (int i = 1; i <= currentImmunityLevel; i++)
        {
            List<Attachment> ringAttachments = GetAttachmentsInRing(i);
            foreach (var attachment in ringAttachments)
            {
                attachment.MakeImmuneToDamage();
            }
        }
    }

    private void UpdateHullSprite()
    {
        if (hullSprites != null && hullSprites.Length > currentImmunityLevel)
        {
            hullSpriteRenderer.sprite = hullSprites[currentImmunityLevel];
        }
        else
        {
            Debug.LogError("Hull sprite array is not properly configured or missing sprites.");
        }
    }

    private List<Attachment> GetAttachmentsInRing(int ringRadius)
    {
        List<Attachment> ringAttachments = new List<Attachment>();
        List<Vector2Int> ringCells = hexGridSystem.GetHexRing(ringRadius);

        foreach (var attachment in attachments)
        {
            if (ringCells.Contains(attachment.gridPosition))
            {
                ringAttachments.Add(attachment);
            }
        }

        return ringAttachments;
    }

    void OnHullDestroyed()
    {
        Debug.Log("Hull Destroyed!");
        Destroy(gameObject);
    }
}