using UnityEngine;

public class Attachment : MonoBehaviour
{
    public Vector2Int gridPosition; // Position of the attachment on the hex grid
    private bool isImmuneToDamage = false; // Whether the attachment is immune to damage
    private Health healthComponent;

    void Start()
    {
        // Get the Health component attached to the same GameObject
        healthComponent = GetComponent<Health>();
    }

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
        if (healthComponent != null)
        {
            healthComponent.SetImmune(isImmuneToDamage); // Update the Health component
        }
    }
}