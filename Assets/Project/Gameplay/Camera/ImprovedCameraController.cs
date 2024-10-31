using Cinemachine;
using UnityEngine;

namespace Project.Gameplay.Camera
{
    public class ImprovedCameraController : MonoBehaviour
    {
        [Header("Camera References")]
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
    
        [Header("Rotation Settings")]
        [SerializeField] private float rotationSpeed = 3f;
        [SerializeField] private bool invertRotation = false;
        [SerializeField] private float maxVerticalAngle = 80f;
        [SerializeField] private float minVerticalAngle = -60f;

        [Header("Follow Settings")]
        [SerializeField] private float followDamping = 0.5f;
        [SerializeField] private Vector3 shoulderOffset = new Vector3(0.5f, 1.5f, -3f);
        [SerializeField] private float cameraDistance = 5f;

        private CinemachineFramingTransposer _framingTransposer;
        private Cinemachine3rdPersonFollow _thirdPersonFollow;
        private Vector3 _currentRotation;
        private Transform _cameraTransform;

        private void Start()
        {
            if (virtualCamera == null)
            {
                virtualCamera = GetComponent<CinemachineVirtualCamera>();
            }

            // Get or add the 3rd person follow component
            _thirdPersonFollow = virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
            if (_thirdPersonFollow == null)
            {
                _thirdPersonFollow = virtualCamera.AddCinemachineComponent<Cinemachine3rdPersonFollow>();
            }

            // Configure the follow settings
            _thirdPersonFollow.ShoulderOffset = shoulderOffset;
            _thirdPersonFollow.CameraDistance = cameraDistance;
            _thirdPersonFollow.Damping = new Vector3(followDamping, followDamping, followDamping);
            _thirdPersonFollow.CameraCollisionFilter = LayerMask.GetMask("Default");

            _cameraTransform = virtualCamera.transform;
            _currentRotation = _cameraTransform.eulerAngles;
        }

        private void LateUpdate()
        {
            HandleCameraRotation();
        }

        private void HandleCameraRotation()
        {
            // Get mouse input for rotation
            float mouseX = UnityEngine.Input.GetAxis("Mouse X") * rotationSpeed * (invertRotation ? -1 : 1);
            float mouseY = UnityEngine.Input.GetAxis("Mouse Y") * rotationSpeed * (invertRotation ? -1 : 1);

            // Update rotation values
            _currentRotation.x -= mouseY; // Vertical rotation
            _currentRotation.y += mouseX; // Horizontal rotation

            // Clamp vertical rotation
            _currentRotation.x = Mathf.Clamp(_currentRotation.x, minVerticalAngle, maxVerticalAngle);

            // Apply rotation to the camera
            _cameraTransform.rotation = Quaternion.Euler(_currentRotation);
        }

        public void SetTarget(Transform target)
        {
            if (virtualCamera != null)
            {
                virtualCamera.Follow = target;
                virtualCamera.LookAt = target;
            }
        }

        // Method to update camera distance at runtime if needed
        public void UpdateCameraDistance(float newDistance)
        {
            if (_thirdPersonFollow != null)
            {
                _thirdPersonFollow.CameraDistance = newDistance;
            }
        }

        // Method to update shoulder offset at runtime if needed
        public void UpdateShoulderOffset(Vector3 newOffset)
        {
            if (_thirdPersonFollow != null)
            {
                _thirdPersonFollow.ShoulderOffset = newOffset;
            }
        }
    }
}