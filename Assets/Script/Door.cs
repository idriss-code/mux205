using Unity.VisualScripting;
using UnityEngine;
enum DoorStatus
{
    Open,
    Close,
    Opening,
    Closing,
}

public class Door : MonoBehaviour
{

    [SerializeField] private Vector3 _openPosition;
    [SerializeField] private Vector3 _closePosition;
    [SerializeField] private DoorStatus _status = DoorStatus.Close;
    [SerializeField] private float _animeTime = 1;
    [SerializeField] private float _shakeIntensinty = 0;
    private float _elapsedTime;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _elapsedTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        switch (_status)
        {
            case DoorStatus.Opening:
                Animate(_openPosition,_closePosition);
                break;
            case DoorStatus.Closing:
                Animate(_closePosition, _openPosition);
                break;
            default:
                StopSound();
                break;
        }
    }

    public void Open()
    {
        PlaySound();
        _status = DoorStatus.Opening;
    }

    public void Close()
    {
        PlaySound();
        _status = DoorStatus.Closing;
    }

    private void Animate(Vector3 startPos, Vector3 endPos)
    {
        _elapsedTime += Time.deltaTime;
        Vector3 pos = Vector3.Lerp(startPos, endPos, _elapsedTime / _animeTime);

        if(_shakeIntensinty != 0)
        {
            pos = new Vector3(
                Random.Range(pos.x + _shakeIntensinty, pos.x - _shakeIntensinty), 
                Random.Range(pos.y + _shakeIntensinty, pos.y - _shakeIntensinty), 
                Random.Range(pos.z + _shakeIntensinty, pos.z - _shakeIntensinty)
                );
        }
        

        GetComponent<Transform>().position = pos;

        if(_elapsedTime  >= _animeTime)
        {
            _elapsedTime = 0;
            _status = _status == DoorStatus.Open ? DoorStatus.Open : DoorStatus.Close;
        }
    }

    private void PlaySound()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        SoudManager soundManager = FindFirstObjectByType<SoudManager>();
        audioSource.clip = soundManager.getDoorSound();
        audioSource.loop = true;
        audioSource.Play();
    }

    private void StopSound()
    {
        AudioSource audioSource = GetComponent<AudioSource>();
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}
