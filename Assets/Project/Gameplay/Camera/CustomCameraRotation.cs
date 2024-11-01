using UnityEngine;

namespace Project.Gameplay.Camera
{
    public class CustomCameraRotation : MonoBehaviour
    {
        public float rotationSpeed = 100f;

        void Update()
        {
            var horizontalInput = UnityEngine.Input.GetAxis("PlayerID_CameraRotationAxis");
            if (horizontalInput != 0) transform.Rotate(Vector3.up, horizontalInput * rotationSpeed * Time.deltaTime);
        }
    }
}
