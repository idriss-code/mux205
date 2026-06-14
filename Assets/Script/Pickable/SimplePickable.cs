using UnityEngine;

public class SimplePickable : MonoBehaviour, IPickable
{
    [SerializeField] private AudioClip _throwSound;

    public AudioClip GetThrowSound()
    {
        return _throwSound;
    }
}
