using UnityEngine;
using static UnityEngine.LowLevelPhysics2D.PhysicsShape;

public class Barrel : MonoBehaviour, IPickable
{
    private SoudManager _soundManager;

    private void Start()
    {
        _soundManager = FindFirstObjectByType<SoudManager>();
    }

    [SerializeField] private GameObject _particlePrefab;
    public AudioClip GetAudioClip()
    {
        return _soundManager.GetKeySound();
    }

    public void Pickup()
    {

    }

    public void Explode()
    {
        GameObject fx = Instantiate(
        _particlePrefab,
        transform.position,
        Quaternion.identity
        );

        // Destruction automatique après 3 secondes
        Destroy(fx, 3f);
        Destroy(gameObject);
    }
}

