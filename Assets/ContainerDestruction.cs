using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

public class BarrelDestruction : MonoBehaviour
{
    public GameObject brokenBarrelPrefab;
    public GameObject deathFeedbackPrefab;
    public float transitionDuration = 0.5f; // Duration for fading effect

    Health _health;
    Renderer _renderer;
    private MMLootGameObject _lootSpawner; 

    void Awake()
    {
        _health = GetComponent<Health>();
        _renderer = GetComponent<Renderer>();
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
