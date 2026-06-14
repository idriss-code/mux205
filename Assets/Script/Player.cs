using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _lookAction;
    private InputAction _attackAction;
    private InputAction _interactAction;
    private AudioSource _audioSource;
    private CharacterController _controller;

    private Vector3 _direction = Vector3.zero;
    private Vector2 _rotation = Vector3.zero;

    private GameObject _tranportedItem = null;

    [SerializeField] private float _movingSpeed = 10.0f;
    [SerializeField] private float _rotationSpeed = 10.0f;
    [SerializeField] private float _throwForce = 500.0f;
    [SerializeField] private float _authorizedForwardAngle = 70.0f;

    [SerializeField] private float _jumpForce = 8.0f;
    [SerializeField] private float _jumpCutMultiplier = 0.4f; // force coupée si on relâche
    [SerializeField] private float _fallMultiplier = 2.5f;    // chute plus rapide
    [SerializeField] private float _airControl = 0.3f;

    private bool _isJumping = false;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _controller = GetComponent<CharacterController>();
        _moveAction = InputSystem.actions.FindAction("Move");
        _jumpAction = InputSystem.actions.FindAction("Jump");
        _lookAction = InputSystem.actions.FindAction("Look");
        _attackAction = InputSystem.actions.FindAction("Attack");
        _interactAction = InputSystem.actions.FindAction("Interact");
        _audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        Vector2 moveValue = _moveAction.ReadValue<Vector2>();
        // ← Crée un forward/right plat (Y=0), ignorant le pitch
        Vector3 flatForward = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
        Vector3 flatRight = new Vector3(transform.right.x, 0, transform.right.z).normalized;

        if (_controller.isGrounded)
        {
            _isJumping = false;

            // ← Mouvement basé sur les axes plats, pas sur transform.TransformDirection
            _direction = flatRight * moveValue.x + flatForward * moveValue.y;
            _direction *= _movingSpeed;

            if (_jumpAction.WasPressedThisFrame())
            {
                _direction.y = _jumpForce;
                _isJumping = true;
            }
        }
        else
        {
            // Contrôle aérien — même correction
            Vector3 airMove = flatRight * moveValue.x + flatForward * moveValue.y;
            _direction.x = Mathf.Lerp(_direction.x, airMove.x * _movingSpeed, _airControl);
            _direction.z = Mathf.Lerp(_direction.z, airMove.z * _movingSpeed, _airControl);

            if (_isJumping && !_jumpAction.IsPressed() && _direction.y > 0)
                _direction.y += Physics.gravity.y * _jumpCutMultiplier * Time.deltaTime;
            else if (_direction.y < 0)
                _direction.y += Physics.gravity.y * _fallMultiplier * Time.deltaTime;
        }

        _direction.y += Physics.gravity.y * Time.deltaTime;
        _controller.Move(_direction * Time.deltaTime);

        Vector2 lookValue = _lookAction.ReadValue<Vector2>();
        _rotation.x -= lookValue.y * _rotationSpeed * Time.deltaTime;
        _rotation.y += lookValue.x * _rotationSpeed * Time.deltaTime;
        _rotation.x = Mathf.Clamp(_rotation.x, -90, 90);
        transform.rotation = Quaternion.Euler(_rotation);

        if (_tranportedItem != null && _attackAction.WasPressedThisFrame() && IsForwardOk())
        {
            ThrowTransportedItem();
        }

        if (_interactAction.WasPressedThisFrame())
        {
            //todo
        }
    }

    private void ThrowTransportedItem()
    {
        PlayTransportedItemSound();

        _tranportedItem.transform.SetLocalPositionAndRotation(new Vector3(0, 0, 2), Quaternion.identity);

        Collider col = _tranportedItem.GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = true;
        }

        Rigidbody rb = _tranportedItem.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = true;
            rb.AddForce(transform.forward * _throwForce);
        }

        _tranportedItem.transform.SetParent(null, true);
        _tranportedItem = null;
    }

    private void PlayTransportedItemSound()
    {
        _audioSource.clip = _tranportedItem.GetComponent<IPickable>().GetThrowSound();
        _audioSource.Play();
    }

    void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;
        IPickable pickable = other.GetComponent<IPickable>();
        if (pickable != null)
        {
            Collider col = other.GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = false;
            }

            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = false;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            _tranportedItem = other.GameObject();
            _tranportedItem.transform.parent = transform;
            _tranportedItem.transform.SetLocalPositionAndRotation(new Vector3(0, -0.75f, 1), Quaternion.identity);
        }
    }

    private bool IsForwardOk()
    {
        return Vector3.Angle(transform.forward, Vector3.down) > _authorizedForwardAngle;
    }

    public KeyCode? GetKeyCode()
    {
        if (_tranportedItem == null) return null;
        Key key = _tranportedItem.GetComponent<Key>();
        if (key == null) return null;

        return key.keyCode;
    }

    public void destroyPickerObject()
    {
        Destroy(_tranportedItem);
        Debug.Assert(_tranportedItem != null);
    }

    public void Kill()
    {
        LevelManager levelManager = FindFirstObjectByType<LevelManager>();
        levelManager.RestartLevel("you die");
    }

    public void MoovToPosition(Transform point)
    {
        if (_controller == null)
        {
            _controller = GetComponent<CharacterController>();
        }

        _controller.enabled = false;
        transform.SetPositionAndRotation(
            point.position,
            point.rotation
         );
        _controller.enabled = true;
    }
}
