using System;
using UnityEngine;

namespace Project.Gameplay.Combat.Shields
{
    public class ShieldProtectionArea : MonoBehaviour
    {
        [Tooltip("The angle within which the shield blocks damage")]
        public float BlockAngle = 90f; // Blocking arc in degrees

        [Tooltip("The forward direction of the shield (relative to its rotation)")]
        public Transform ShieldForward;

        [Tooltip("Is the shield currently raised?")]
        public bool ShieldIsActive;

        void Start()
        {
            OnShieldEquipped?.Invoke(this);
        }

        public static event Action<ShieldProtectionArea> OnShieldEquipped;

        public bool IsBlocking(Vector3 attackPosition)
        {
            if (!ShieldIsActive) return false;

            if (ShieldForward == null) ShieldForward = transform; // Default to the shield's transform

            // Calculate the direction from the attack to the shield
            var attackDirection = (attackPosition - transform.position).normalized;

            // Check the angle between the attack direction and the shield's forward direction
            var angle = Vector3.Angle(ShieldForward.forward, attackDirection);

            // If BlockAngle is 360, always block; otherwise, compare
            return BlockAngle >= 360 || angle <= BlockAngle / 2;
        }
    }
}
