using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeLevelTrigger : MonoBehaviour
{
    [SerializeField] private string _targetSceneName;
    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            LevelManager levelManager = FindFirstObjectByType<LevelManager>();
            levelManager.ChangeLevel(_targetSceneName);
        }
    }
}
