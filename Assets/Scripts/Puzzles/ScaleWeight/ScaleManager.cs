using System;
using System.Collections.Generic;
using UnityEngine;

public class ScaleManager : MonoBehaviour
{
    [HideInInspector]
    public List<OnScaleActions> onScaleActions;

    private void Awake()
    {
        foreach (var action in onScaleActions)
            action.plateInteractAction += OnInteractMethod;
    }

    private void OnInteractMethod(Plate plate)
    {
        if ()

        if (plate == Plate.Left)
        {
            ObjectCreator.Instance.InstantiateObject();
        }
        else
        {

        }
    }
}
