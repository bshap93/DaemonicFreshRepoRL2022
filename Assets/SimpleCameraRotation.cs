using Cinemachine;
using UnityEngine;

public class SimpleCameraRotation : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera;
    public float rotationSpeed = 100f;

    void Update()
    {
        // Assuming Player1_CameraRotationAxis is mapped to the right joystick or horizontal keys
        var horizontalInput = Input.GetAxis("Player1_CameraRotationAxis");
        if (horizontalInput != 0)
            virtualCamera.transform.Rotate(Vector3.up, horizontalInput * rotationSpeed * Time.deltaTime);
    }
}
