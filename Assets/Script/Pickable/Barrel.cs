using UnityEngine;

public class Barrel : MonoBehaviour, IPickable
{
    [SerializeField] private AudioClip _throwSound;
    [SerializeField] private GameObject _particlePrefab;


    public AudioClip GetThrowSound()
    {
        return _throwSound;
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

