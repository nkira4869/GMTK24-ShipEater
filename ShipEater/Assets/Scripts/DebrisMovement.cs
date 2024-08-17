using UnityEngine;

public class DebrisMovement : MonoBehaviour
{
    public float moveSpeed = 2f; // Speed at which the debris moves downwards
    private bool isAttached = false; // Tracks whether the debris is attached to the ship

    void Update()
    {
        // Move the debris downwards only if it is not attached
        if (!isAttached)
        {
            transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);
        }
    }

    // This method can be called by the DebrisAttachment script to stop the movement when attached
    public void AttachToShip()
    {
        isAttached = true;
    }
}
