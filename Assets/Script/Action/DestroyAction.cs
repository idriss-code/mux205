using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class DestroyAction : MonoBehaviour, IAction
{
    [SerializeField] private GameObject[] _targets = null;
    [SerializeField] private GameObject _particlePrefab = null;

    public void Go()
    {


        foreach (var target in _targets) {
            if (target != null && target.activeInHierarchy)
            {
                target.SetActive(false);
                ShowParticle(target.transform);
            }
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
