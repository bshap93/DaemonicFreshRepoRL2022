using UnityEngine;

public class DebugCollisions : MonoBehaviour
{
// Add this temporary debug code to your player/enemy scripts
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log(
            $"Collision with {collision.gameObject.name} on layer {LayerMask.LayerToName(collision.gameObject.layer)}");
    }

    void OnTriggerEnter(Collider other)
    {
    }
}
