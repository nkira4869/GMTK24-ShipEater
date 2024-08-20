using UnityEngine;

public class Attachment : MonoBehaviour
{
    public Vector2Int gridPosition; // Position of the attachment on the hex grid
    private bool isImmuneToDamage = false; // Whether the attachment is immune to damage
    private Health healthComponent;
    private DynamicHexGrid dynamicHexGrid;

    void Start()
    {
        // Get the Health component attached to the same GameObject
        healthComponent = GetComponent<Health>();
        dynamicHexGrid = FindObjectOfType<DynamicHexGrid>(); // Get reference to the new DynamicHexGrid system
    }

    // Method to attach the object to the grid
    public void Attach(DynamicHexGrid hexGridSystem)
    {
        // Get the grid position using the updated grid system
        gridPosition = hexGridSystem.WorldToHexCoordinates(transform.position);
        hexGridSystem.OccupyCell(gridPosition); // Mark the cell as occupied
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

    // Remove immunity from the attachment
    public void RemoveImmunity()
    {
        isImmuneToDamage = false;
        if (healthComponent != null)
        {
            healthComponent.SetImmune(isImmuneToDamage); // Update the Health component
        }
    }
}