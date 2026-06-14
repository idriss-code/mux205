using UnityEngine;

public class MakeAppearAction : MonoBehaviour, IAction
{
    [SerializeField] private GameObject _target = null;
    [SerializeField] private GameObject _particlePrefab = null;

    private AudioSource _audioSource;


    void Start()
    {
        _audioSource = _target.GetComponent<AudioSource>();
    }

    public void Go()
    {
        if (_target != null)
        {
            _target.SetActive(true);
            PlaySound();
            ShowParticle(_target.transform);
        }
    }

    private void PlaySound()
    {
        if (_audioSource != null)
        {
            _audioSource.loop = false;
            _audioSource.Play();
        }
    }

    private void ShowParticle(Transform pTransform)
    {
        if (_particlePrefab != null)
        {
            GameObject fx = Instantiate(
            _particlePrefab,
            pTransform.position,
            Quaternion.identity
            );

            Destroy(fx, 3f);
        }
    }
}
