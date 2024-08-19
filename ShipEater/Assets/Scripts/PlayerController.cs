using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float horizontalSpeed = 5f; // Speed for left and right movement
    public float verticalSpeed = 3f;   // Speed for up and down movement

    private Vector2 movementInput; // Store the movement input
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main; // Reference the main camera
    }

    void Update()
    {
        // Capture input from the arrow keys or WASD
        movementInput.x = Input.GetAxisRaw("Horizontal"); // Left and right movement
        movementInput.y = Input.GetAxisRaw("Vertical");   // Up and down movement

        // Move the player based on input, with separate speeds for each axis
        Vector3 newPosition = transform.position + new Vector3(movementInput.x * horizontalSpeed, movementInput.y * verticalSpeed, 0f) * Time.deltaTime;

        // Clamp the player's position within the camera bounds
        newPosition = ClampPositionToCameraBounds(newPosition);

        // Apply the clamped position
        transform.position = newPosition;
    }

    Vector3 ClampPositionToCameraBounds(Vector3 targetPosition)
    {
        // Calculate the screen boundaries in world space
        float vertExtent = mainCamera.orthographicSize;
        float horzExtent = vertExtent * Screen.width / Screen.height;

        // Get the boundaries of the camera view
        float minX = mainCamera.transform.position.x - horzExtent;
        float maxX = mainCamera.transform.position.x + horzExtent;
        float minY = mainCamera.transform.position.y - vertExtent;
        float maxY = mainCamera.transform.position.y + vertExtent;

        // Clamp the target position within the camera bounds
        targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);

        return targetPosition;
    }

    // Method to apply a horizontal speed modifier
    public void ApplyHorizontalSpeedModifier(float modifier)
    {
        horizontalSpeed += (horizontalSpeed * modifier) / 100;
    }

    // Method to apply a vertical speed modifier
    public void ApplyVerticalSpeedModifier(float modifier)
    {
        verticalSpeed += (verticalSpeed * modifier) / 100;
    }
}