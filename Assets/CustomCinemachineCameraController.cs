using MoreMountains.TopDownEngine;
using UnityEngine;

public class CustomCinemachineCameraController : CinemachineCameraController
{
    public float rotationSpeed = 5f;
    Vector3 _lastPosition;
    Transform _rotatingChild;

    protected override void Start()
    {
        base.Start();
        InitializeRotatingChild();
    }

    void Update()
    {
        if (_rotatingChild == null) return;

        // Calculate the movement direction of the rotating child object
        var movementDirection = _rotatingChild.position - _lastPosition;

        if (movementDirection.magnitude > 0.1f)
        {
            // Calculate the target rotation based on the rotating child's movement direction
            var targetRotation = Quaternion.LookRotation(movementDirection, Vector3.up);

            // Smoothly interpolate the camera rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Update last position for the next frame
        _lastPosition = _rotatingChild.position;
    }

    public override void SetTarget(Character character)
    {
        base.SetTarget(character);
        InitializeRotatingChild();
    }

    void InitializeRotatingChild()
    {
        if (TargetCharacter != null && TargetCharacter.CharacterModel != null)
        {
            _rotatingChild = TargetCharacter.CharacterModel.transform;
            _lastPosition = _rotatingChild.position;
        }
    }
}
