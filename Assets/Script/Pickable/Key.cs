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
    [SerializeField] private AudioClip _throwSound;
    [SerializeField] public KeyCode keyCode = KeyCode.Red;

    public AudioClip GetThrowSound() 
    {
        return _throwSound;
    }
}
