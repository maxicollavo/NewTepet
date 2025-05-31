using System;
using System.Collections.Generic;
using UnityEngine;

public class ScaleManager : MonoBehaviour
{
    [HideInInspector]
    public List<OnScaleActions> onScaleActions;

    public Transform[] spawnPos;

    private void Awake()
    {
        foreach (var action in onScaleActions)
            action.plateInteractAction += OnInteractMethod;
    }

    private void OnInteractMethod(Plate plate)
    {
        if (!HandInventory.hasObjInHand) return;

        if (plate == Plate.Left)

        {
            //ObjectCreator.Instance.InstantiateObject(PickedObjData.Instance.CurrentPickedObj, spawnPos[0].position);
        }
        else
        {

        }
    }
}
