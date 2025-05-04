using UnityEngine;

public class LaserReceptor : MonoBehaviour, Interactor
{
    public void Aiming()
    {
        Debug.Log("Aim");
    }

    public void DisableOutline()
    {
        Debug.Log("Dis");
    }

    public void Interact()
    {
        Debug.Log("interactua laser");
    }
}
