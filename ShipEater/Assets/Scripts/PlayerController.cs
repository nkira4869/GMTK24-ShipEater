using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float horizontalSpeed = 5f; // Speed for left and right movement
    public float verticalSpeed = 3f;   // Speed for up and down movement

    private Vector2 movementInput; // Store the movement input

    void Update()
    {
        // Capture input from the arrow keys or WASD
        movementInput.x = Input.GetAxisRaw("Horizontal"); // Left and right movement
        movementInput.y = Input.GetAxisRaw("Vertical");   // Up and down movement

        // Move the player based on input, with separate speeds for each axis
        transform.position += new Vector3(movementInput.x * horizontalSpeed, movementInput.y * verticalSpeed, 0f) * Time.deltaTime;
    }
}
