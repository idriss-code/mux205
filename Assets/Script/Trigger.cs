using UnityEngine;

public class Trigger : MonoBehaviour
{
    [SerializeField] private KeyCode keyCode = KeyCode.Red;
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
        return key != null && key.keyCode.Equals(this.keyCode);
    }

    private bool IsPlayerPlayerTriggerOk(Player player)
    {
        return player != null && player.GetKeyCode() != null && player.GetKeyCode().Equals(this.keyCode);
    }

    private void Action()
    {
        IAction action = GetComponent<IAction>();
        if (action != null)
        {
            action.Go();
        }
    }

}
