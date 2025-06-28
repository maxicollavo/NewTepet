using System.Collections.Generic;
using UnityEngine;

public class ChangeObjectLayer : MonoBehaviour
{
    GameObject[] inventoryObjChildrens;

    private void Start()
    {
        Transform[] children = this.GetComponentsInChildren<Transform>(true);
        var childList = new List<GameObject>();

        foreach (Transform t in children)
        {
            if (t.gameObject != this.gameObject)
                childList.Add(t.gameObject);
        }

        inventoryObjChildrens = childList.ToArray();
    }

    public void ChangeBasePyramidLayer()
    {
        int pyramidLayer = LayerMask.NameToLayer("Default");

        this.gameObject.layer = pyramidLayer;

        foreach (GameObject child in inventoryObjChildrens)
        {
            child.layer = pyramidLayer;
        }
    }
}
