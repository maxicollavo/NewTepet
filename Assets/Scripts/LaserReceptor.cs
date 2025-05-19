using UnityEngine;

public class LaserReceptor : MonoBehaviour, Interactor
{
    [SerializeField] RotateSphere sphere;

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
        sphere.SetOnWinSphereMaterials();
    }
}
