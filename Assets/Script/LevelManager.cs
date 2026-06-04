using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private string _currentLevel = "level1";

    [SerializeField]
    private float _fadeDuration = 0.8f;

    [SerializeField]
    private AnimationCurve _fadeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [SerializeField]
    private TextMeshProUGUI _textMesh = null;

    private bool _isLoading = false;

    private GameObject _player;
    private Volume _volume;


    private void Start()
    {
        if( _textMesh  == null)
        {
            Debug.LogError("text mesh is null");
        }

        _player = GameObject.Find("Player");

        if( _player == null)
        {
            Debug.LogError("Player is null");
        }

        GameObject pp = GameObject.Find("---PostProcessing---");
        _volume = pp.GetComponent<Volume>();
        _volume.weight = 1.0f;

        StartLevel();
    }

    public void ChangeLevel(string newLevel)
    {
        if (_isLoading)
            return;

        _isLoading = true;

        StartCoroutine(ChangeLevelCoroutine(newLevel));
    }

    public void RestartLevel(string message = null)
    {
        if (_isLoading)
            return;

        _isLoading = true;

        StartCoroutine(ChangeLevelCoroutine(_currentLevel, message));
    }

    public void StartLevel()
    {
        if (_isLoading)
            return;

        _isLoading = true;

        StartCoroutine(StartLevelCoroutine(_currentLevel));
    }

    private IEnumerator ChangeLevelCoroutine(string newLevel, string message = null)
    {
        if (message != null)
        {
            _textMesh.text = message;
        }
        yield return StartCoroutine(FadeIn());

        yield return SceneManager.UnloadSceneAsync(_currentLevel);

        yield return StartCoroutine(StartLevelCoroutine(newLevel));
    }

    private IEnumerator StartLevelCoroutine(string newLevel)
    {
        _textMesh.text = newLevel;

        yield return SceneManager.LoadSceneAsync(
            newLevel,
            LoadSceneMode.Additive
        );

        PostLoading(newLevel);

        yield return StartCoroutine(FadeOut());

        _textMesh.text = null;
    }

    private void PostLoading(string newLevel)
    {
        Scene loadedScene = SceneManager.GetSceneByName(newLevel);

        if (loadedScene.IsValid())
        {
            SceneManager.SetActiveScene(loadedScene);
        }

        _currentLevel = newLevel;

        MoovPlayerToSpawn();

        _isLoading = false;
    }

    private void MoovPlayerToSpawn()
    {
        GameObject spawn = GameObject.Find("spawn");
        if (spawn == null)
        {
            Debug.LogError("spawn is null");
            return;
        }

        _player.GetComponent<Player>().MoovToPosition(spawn.transform);
    }

    private IEnumerator FadeOut()
    {
        float elapsed = 0f;
        float startWeight = _volume.weight;

        while (elapsed < _fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = _fadeCurve.Evaluate(Mathf.Clamp01(elapsed / _fadeDuration));

            _volume.weight = Mathf.Lerp(startWeight, 0f, t);

            yield return null;
        }

        _volume.weight = 0f;
    }

    private IEnumerator FadeIn()
    {
        float elapsed = 0f;

        while (elapsed < _fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = _fadeCurve.Evaluate(Mathf.Clamp01(elapsed / _fadeDuration));

            _volume.weight = Mathf.Lerp(0f, 1f, t);

            yield return null;
        }

        _volume.weight = 1f;
    }
}