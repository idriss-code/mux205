using UnityEngine;

public class Destructible : MonoBehaviour
{
    [SerializeField] private GameObject _particlePrefab;
    [SerializeField] private float _explosionDispertion = 10.0f;
    [SerializeField] private int _explosionQty = 10;
    [SerializeField] private Color[] _colors;
    private int _pv;

    private IAction[] _actions;

    private void Start()
    {
        _pv = _colors.Length;
        _actions = GetComponents<IAction>();
    }

    void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;
        Barrel barrel = other.GetComponent<Barrel>();
        if (barrel != null)
        {
            barrel.Explode();
            if (_pv <= 0)
            {
                Explode();
                return;
            }

            ChangeColor(getCurrentColor());
            _pv--;
        }
    }

    private Color getCurrentColor()
    {
        return _colors[_colors.Length - _pv];
    }

    private void ChangeColor(Color color)
    {
        MeshRenderer gameObjectRenderer = GetComponent<MeshRenderer>();
        gameObjectRenderer.material.color = color;
    }

    public void Explode()
    {
        for (int i = 0; i < _explosionQty; i++)
        {
            CreateExplosion();
        }

        Action();
        Destroy(gameObject);
    }

    private void CreateExplosion()
    {
        GameObject fx = Instantiate(
        _particlePrefab,
        transform.position + new Vector3(
            Random.Range(-_explosionDispertion, _explosionDispertion),
            Random.Range(-_explosionDispertion, _explosionDispertion),
            Random.Range(-_explosionDispertion, _explosionDispertion)
            ),
        Quaternion.identity
        );

        // Destruction automatique après 3 secondes
        Destroy(fx, 3f);
    }

    private void Action()
    {
        foreach (IAction action in _actions)
        {
            action.Go();
        }
    }
}
