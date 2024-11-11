// Attach this script to the projectile

using UnityEngine;

public class ProjectileDirectionFix : MonoBehaviour
{
    void Start()
    {
        // Correct the rotation to match the weapon's forward direction
        transform.forward = transform.parent.forward;
    }
}
