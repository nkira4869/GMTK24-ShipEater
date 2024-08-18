using UnityEngine;

public class Attachment : MonoBehaviour
{
    public Vector2Int gridPosition; // Position of the attachment on the hex grid
    private bool isImmuneToDamage = false; // Whether the attachment is immune to damage

    // Method to attach the object to the grid
    public void Attach(ShipHexGridSystem hexGridSystem)
    {
        gridPosition = hexGridSystem.GetAxialCoordinates(transform.position);
        hexGridSystem.MarkCellAsOccupied(gridPosition);
    }

    // Make the attachment immune to damage
    public void MakeImmuneToDamage()
    {
        isImmuneToDamage = true;
    }

    // Method to take damage
    public void TakeDamage(float damage)
    {
        if (!isImmuneToDamage)
        {
            // Handle damage logic here (e.g., reduce health, destroy the attachment)
            Debug.Log($"Attachment at {gridPosition} took {damage} damage.");
        }
    }
}