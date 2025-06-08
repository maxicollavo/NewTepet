using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultManager : MonoBehaviour
{
    [SerializeField] WeightManager weightManager;
    [SerializeField] Animator[] doorAnims;

    bool[] doorsDone;
    int counter;

    private void Awake()
    {
        weightManager.OnResultAction += OnResultMethod;
        doorsDone = new bool[doorAnims.Length];
    }

    private void OnResultMethod(float result)
    {
        int rounded = Mathf.RoundToInt(result);
        switch (rounded)
        {
            case 2:
                OpenDoor();
                Debug.Log("Se abre la puerta con peso 2");
                break;
            case 7:
                OpenDoor();
                Debug.Log("Se abre la puerta con peso 7");
                break;
            case 9:
                OpenDoor();
                Debug.Log("Se abre la puerta con peso 9");
                break;
            case 15:
                OpenDoor();
                Debug.Log("Se abre la puerta con peso 15");
                break;
            case 50:
                OpenDoor();
                Debug.Log("Se abre la puerta con peso 50");
                break;
            default:
                Debug.Log("No se llega a ningun peso");
                break;
        }
    }

    private void OpenDoor()
    {
        if (counter >= doorAnims.Length)
        {
            Debug.LogWarning("No hay más puertas para abrir.");
            return;
        }

        if (doorsDone[counter]) return;
        doorAnims[counter].SetTrigger("Open");
        AudioManager.Instance.PlaySound("rocaMoviendose");
        doorsDone[counter] = true;
        counter++;
    }
}