using UnityEngine;

public class SoudManager : MonoBehaviour
{
    [SerializeField] private AudioClip _keySound;
    [SerializeField] private AudioClip _doorSound;
    [SerializeField] private AudioClip _PortalSound;

    public AudioClip GetKeySound()
    {
        return _keySound;
    }

    public AudioClip GetPortalSound()
    {
        return _PortalSound;
    }

    public  AudioClip getDoorSound()
    {
        return _doorSound;
    }
}
