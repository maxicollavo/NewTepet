using System;
using System.Collections.Generic;
using UnityEngine;

public class ScaleManager : MonoBehaviour
{
    [HideInInspector]
    public List<OnScaleActions> onScaleActions;

    public Transform[] leftSpawnPos;
    public Transform[] rightSpawnPos;

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
            ObjectCreator.Instance.InstantiateObject(PickedObjData.Instance.CurrentPickedObj, leftSpawnPos[UnityEngine.Random.Range(0, leftSpawnPos.Length)].position, plate);
        }
        else
        {
            ObjectCreator.Instance.InstantiateObject(PickedObjData.Instance.CurrentPickedObj, rightSpawnPos[UnityEngine.Random.Range(1, rightSpawnPos.Length)].position, plate);
        }

        PickedObjData.Instance.MarkAsThrowed(PickedObjData.Instance.CurrentPickedObj, true);
    }
}
