using UnityEngine;

public class OpenPortalAction : MonoBehaviour, IAction
{
    [SerializeField] private GameObject portal = null;
    
    public void Go()
    {
        if(portal != null)
        {
            portal.SetActive(true);
            PlaySound();
        }
    }

    private void PlaySound()
    {
        AudioSource audioSource = portal.GetComponent<AudioSource>();
        SoudManager soundManager = FindFirstObjectByType<SoudManager>();
        audioSource.clip = soundManager.GetPortalSound();
        audioSource.loop = false;
        audioSource.Play();
    }
}
