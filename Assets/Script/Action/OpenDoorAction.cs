using UnityEngine;

public class OpenDoorAction : MonoBehaviour, IAction
{
    [SerializeField] private GameObject door = null;
    
    public void Go()
    {
        if(door != null)
        {
            door.GetComponent<Door>().Open();
        }
    }
}
