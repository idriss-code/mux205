using UnityEngine;

public enum KeyCode
{
    Blue,
    Red,
    Yellow,
    Green
}

public class Key : MonoBehaviour, IPickable
{

    private SoudManager _soundManager;

    [SerializeField] public KeyCode keyCode = KeyCode.Red;

    private void Start()
    {
        _soundManager = FindFirstObjectByType<SoudManager>();
    }

    public void Pickup()
    {

    }

    public AudioClip GetAudioClip() 
    { 
        return _soundManager.GetKeySound();
    }
}
