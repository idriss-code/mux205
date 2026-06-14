using UnityEngine;

public class Trigger : MonoBehaviour
{
    [SerializeField] private KeyCode _keyCode = KeyCode.Red;

    private IAction[] _actions;

    private void Start()
    {
        _actions = GetComponents<IAction>();
        if (_actions.Length == 0)
            Debug.LogWarning("Trigger : aucun IAction trouvé sur " + gameObject.name);
    }

    private void OnTriggerEnter(Collider other)
    {
        Key key = other.GetComponent<Key>();
        if (IsTriggerKeyOk(key))
        {
            Action();
            Destroy(key.gameObject);
            return;
        }

        Player player = other.GetComponent<Player>();
        if (IsPlayerPlayerTriggerOk(player))
        {
            Action();
            player.destroyPickerObject();
        }
    }

    private bool IsTriggerKeyOk(Key key)
    {
        return key != null && key.keyCode.Equals(this._keyCode);
    }

    private bool IsPlayerPlayerTriggerOk(Player player)
    {
        return player != null && player.GetKeyCode() != null && player.GetKeyCode().Equals(this._keyCode);
    }

    private void Action()
    {
        foreach (IAction action in _actions)
        {
            action.Go();
        }
    }

}
