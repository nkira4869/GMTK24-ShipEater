using System.Collections.Generic;
using UnityEngine;

public class HullManager : MonoBehaviour
{
    public Health hullHealth; // Reference to the existing Health component
    public ShipHexGridSystem hexGridSystem; // Reference to the hex grid system
    private List<Attachment> attachments = new List<Attachment>(); // List of all attachments

    [Header("Leveling System")]
    public int currentLevel = 1; // Track the current level of the hull
    public int levelUpThreshold = 5; // Number of attachments required to level up
    public float hexRadiusExpansionAmount = 0.5f; // Amount by which the hex radius increases when leveling up
    public int immuneAttachmentsCount = 2; // Number of attachments to make immune when leveling up

    void Start()
    {
        if (hullHealth == null)
        {
            hullHealth = gameObject.GetComponent<Health>(); // Get the Health component if not assigned
        }

        // Subscribe to the health events
        hullHealth.onHealthChanged += OnHealthChanged;
        hullHealth.onDeath += OnHullDestroyed;
    }

    // Track and manage attachments
    public void AddAttachment(Attachment newAttachment)
    {
        attachments.Add(newAttachment);
        newAttachment.Attach(hexGridSystem); // Attach it to the grid

        // Check if we need to level up
        if (attachments.Count % levelUpThreshold == 0)
        {
            LevelUp();
        }
    }

    // Track the number of attachments
    public int GetAttachmentCount()
    {
        return attachments.Count;
    }

    // Track the position of each attachment on the grid
    public List<Vector2Int> GetAttachmentPositions()
    {
        List<Vector2Int> positions = new List<Vector2Int>();
        foreach (var attachment in attachments)
        {
            positions.Add(attachment.gridPosition);
        }
        return positions;
    }

    // Callback for when the hull's health changes
    void OnHealthChanged(float currentHealth, float maxHealth)
    {
        // You can add logic here to respond to health changes if needed (e.g., play effects, UI updates)
        Debug.Log($"Hull Health Changed: {currentHealth}/{maxHealth}");
    }

    // Level up the hull, making some attachments immune and expanding the grid radius
    void LevelUp()
    {
        currentLevel++; // Increase the current level

        // Find the attachments closest to the center of the grid
        List<Attachment> closestAttachments = FindClosestAttachmentsToCenter(attachments, immuneAttachmentsCount);

        // Make the selected attachments immune to damage
        foreach (var attachment in closestAttachments)
        {
            attachment.MakeImmuneToDamage();
        }

        // Expand the grid radius
        //ExpandGridRadius();

        Debug.Log($"Hull leveled up to level {currentLevel}. {immuneAttachmentsCount} attachments are now immune to damage.");
    }

    // Find the closest attachments to the grid center
    List<Attachment> FindClosestAttachmentsToCenter(List<Attachment> allAttachments, int count)
    {
        allAttachments.Sort((a, b) => Vector2Int.Distance(Vector2Int.zero, a.gridPosition).CompareTo(Vector2Int.Distance(Vector2Int.zero, b.gridPosition)));
        return allAttachments.GetRange(0, Mathf.Min(count, allAttachments.Count));
    }

    // Expand the grid by increasing the hex radius
    void ExpandGridRadius()
    {
        hexGridSystem.IncreaseHexRadius(hexRadiusExpansionAmount);

        // Recalculate the positions of all attachments based on the new hex radius
        foreach (var attachment in attachments)
        {
            Vector3 newPosition = hexGridSystem.GetWorldPosition(attachment.gridPosition);
            attachment.transform.position = newPosition;
        }
    }

    // Callback for when the hull is destroyed
    void OnHullDestroyed()
    {
        Debug.Log("Hull Destroyed!");
        // Add destruction logic here (e.g., trigger explosion, end game)
        Destroy(gameObject);
    }
}