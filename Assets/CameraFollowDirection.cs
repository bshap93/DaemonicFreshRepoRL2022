using UnityEngine;

public class CameraFollowDirection : MonoBehaviour
{
    public Transform player; // Reference to the player object
    public float rotationSpeed = 5f; // Speed at which the camera rotates

    Vector3 lastPosition;

    void Start()
    {
        lastPosition = player.position;
    }

    void LateUpdate()
    {
        var playerMovement = player.position - lastPosition;

        if (playerMovement.magnitude > 0.1f) // Check if player has moved
        {
            // Calculate the direction the player is moving
            var direction = playerMovement.normalized;

            // Set the camera's forward direction based on player's movement
            var targetRotation = Quaternion.LookRotation(direction, Vector3.up);

            // Smoothly rotate the camera towards the target direction
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        lastPosition = player.position;
    }
}
