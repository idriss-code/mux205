using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] private float _xRotationSpeed;
    [SerializeField] private float _yRotationSpeed;
    [SerializeField] private float _zRotationSpeed;

    private void FixedUpdate()
    {
        transform.Rotate(
            _xRotationSpeed * Time.deltaTime,
            _yRotationSpeed * Time.deltaTime,
            _zRotationSpeed * Time.deltaTime
        );
    }
}
