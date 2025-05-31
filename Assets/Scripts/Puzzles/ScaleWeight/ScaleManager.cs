using System;
using System.Collections.Generic;
using UnityEngine;

public class ScaleManager : MonoBehaviour
{
    [HideInInspector]
    public List<OnScaleActions> onScaleActions;

    public Transform[] spawnPos;

    void Start()
    {
        foreach (var onScale in onScaleActions)
        {
            onScale.plateInteractAction += OnInteractMethod;
        }
    }

    private void OnInteractMethod(Plate plate, OnScaleActions onScale)
    {
        if (!HandInventory.hasObjInHand)
        {
            StartCoroutine(onScale.CannotEnter());
            return;
        }
        if (PickedObjData.Instance.CurrentPickedObj == ObjectsToPick.BoardPiece || PickedObjData.Instance.CurrentPickedObj == ObjectsToPick.GlassSphere)
        {
            StartCoroutine(onScale.CannotEnter());
            return;
        }

        if (plate == Plate.Left)
        {
            ObjectCreator.Instance.InstantiateObject(PickedObjData.Instance.CurrentPickedObj, spawnPos[0].position, plate);
        }
        else
        {
            ObjectCreator.Instance.InstantiateObject(PickedObjData.Instance.CurrentPickedObj, spawnPos[1].position, plate);
        }

        PickedObjData.Instance.MarkAsThrowed(PickedObjData.Instance.CurrentPickedObj);
    }
}
