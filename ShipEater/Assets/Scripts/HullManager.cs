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
    public float scaleUpAmount = 0.1f; // Amount to scale the hull during level up
    public int immuneAttachmentsCount = 2;

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

        if (attachments.Count % levelUpThreshold == 0)
        {
            LevelUp();
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

        ExpandGridRadiusAndScaleHull();
        Debug.Log($"Hull leveled up to level {currentLevel}. {immuneAttachmentsCount} attachments are now immune to damage.");
    }

    List<Attachment> FindClosestAttachmentsToCenter(List<Attachment> allAttachments, int count)
    {
        allAttachments.Sort((a, b) => Vector2Int.Distance(Vector2Int.zero, a.gridPosition).CompareTo(Vector2Int.Distance(Vector2Int.zero, b.gridPosition)));
        return allAttachments.GetRange(0, Mathf.Min(count, allAttachments.Count));
    }

    void ExpandGridRadiusAndScaleHull()
    {
        // Expand the hex grid radius
        hexGridSystem.IncreaseHexRadius(hexRadiusExpansionAmount);

        // Scale the hull itself
        Vector3 scaleIncrease = new Vector3(scaleUpAmount, scaleUpAmount, 0);
        transform.localScale += scaleIncrease;

        // Normalize the scale of each attachment to prevent exponential growth
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