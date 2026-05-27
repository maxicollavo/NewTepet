using UnityEngine;

public class PlaceObject : MonoBehaviour
{
    [SerializeField] GameObject objDisable;
    [SerializeField] GameObject objEnable;

    public void Place()
    {
        objDisable.SetActive(false);
        objEnable.SetActive(true);
    }
}
