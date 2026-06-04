using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class Destructible : MonoBehaviour
{
    [SerializeField] private GameObject _particlePrefab;
    [SerializeField] private float _explosionDispertion = 10.0f;
    [SerializeField] private int _explosionQty = 10;
    [SerializeField] private Color[] _colors;
    private int _pv; 


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _pv = _colors.Length;
    }

    // Update is called once per frame
    void Update()
    {
        
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

            changeColor(getCurrentColor());
            _pv--;
        }
    }

    private Color getCurrentColor()
    {
        return _colors[_colors.Length - _pv];
    }

    private void changeColor(Color color)
    {
        MeshRenderer gameObjectRenderer = GetComponent<MeshRenderer>();
        Material newMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        newMaterial.color = color;
        gameObjectRenderer.material = newMaterial;
    }

    public void Explode()
    {
        for (int i = 0; i < _explosionQty; i++)
        {
            CreateExplosion();
        }

        OnDestroyAction();
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

    private void OnDestroyAction()
    {
        IAction action = GetComponent<IAction>();
        if (action != null)
        {
            action.Go();
        }
    }
}
