using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultManager : MonoBehaviour
{
    [SerializeField] WeightManager weightManager;

    private void Awake()
    {
        weightManager.OnResultAction += OnResultMethod;
    }

    private void OnResultMethod(float result)
    {
        int rounded = Mathf.RoundToInt(result);
        switch (rounded)
        {
            case 2:
                Debug.Log("Se abre la puerta con peso 2");
                break;
            case 7:
                Debug.Log("Se abre la puerta con peso 7");
                break;
            case 9:
                Debug.Log("Se abre la puerta con peso 9");
                break;
            case 15:
                Debug.Log("Se abre la puerta con peso 15");
                break;
            case 50:
                Debug.Log("Se abre la puerta con peso 50");
                break;
            default:
                Debug.Log("No se llega a ningun peso");
                break;
        }
    }
}