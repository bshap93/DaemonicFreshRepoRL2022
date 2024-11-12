using MoreMountains.TopDownEngine;
using UnityEngine;

public class BarrelDestruction : MonoBehaviour
{
    public GameObject brokenBarrelPrefab; // Broken barrel prefab
    public GameObject deathFeedbackPrefab; // Temporary feedback prefab

    Health _health;

    void Awake()
    {
        _health = GetComponent<Health>();
        _health.OnDeath += OnDeath;
    }

    void OnDestroy()
    {
        _health.OnDeath -= OnDeath;
    }

    void OnDeath()
    {
        // Spawn the temporary feedback object at the barrel's position
        Instantiate(deathFeedbackPrefab, transform.position, transform.rotation);

        // Instantiate the broken barrel at the same position
        Instantiate(brokenBarrelPrefab, transform.position, transform.rotation);

        // Destroy the original barrel
        Destroy(gameObject);
    }
}
