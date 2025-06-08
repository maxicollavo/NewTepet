using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GarbageManager : MonoBehaviour
{
    [SerializeField] OnScaleActions onScaleActions;

    [SerializeField] Transform spawnPoint;

    private void Awake()
    {
        onScaleActions.garbageInteractAction += OnInteractMethod;
    }

    private void OnInteractMethod(OnScaleActions actions)
    {
        if (!HandInventory.hasObjInHand)
        {
            StartCoroutine(actions.CannotEnter());
            return;
        }
        if (PickedObjData.Instance.CurrentPickedObj == ObjectsToPick.BoardPiece || PickedObjData.Instance.CurrentPickedObj == ObjectsToPick.GlassSphere)
        {
            StartCoroutine(actions.CannotEnter());
            return;
        }

        ObjectCreator.Instance.InstantiateObject(PickedObjData.Instance.CurrentPickedObj, spawnPoint.position, Plate.None);
        PickedObjData.Instance.MarkAsThrowed(PickedObjData.Instance.CurrentPickedObj, true);
    }
}
