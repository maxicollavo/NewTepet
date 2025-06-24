using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultManager : MonoBehaviour
{
    [SerializeField] WeightManager weightManager;
    [SerializeField] Animator[] doorAnims;
    [SerializeField] AudioSource[] doorSounds3D;
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
                TryOpenDoor(0);
                break;
            case 7:
                TryOpenDoor(1);
                break;
            case 9:
                TryOpenDoor(2);
                break;
            case 15:
                TryOpenDoor(3);
                break;
            case 50:
                TryOpenDoor(4);
                break;
            default:
                Debug.Log("No se llega a ningun peso");
                break;
        }
    }

    private void TryOpenDoor(int doorIndex)
    {
        if (doorIndex >= doorAnims.Length)
        {
            Debug.LogWarning("Índice de puerta fuera de rango.");
            return;
        }

        if (doorsDone[doorIndex]) return;

        doorAnims[doorIndex].SetTrigger("Open");
        doorSounds3D[doorIndex].Play();
        doorsDone[doorIndex] = true;
        Debug.Log($"Se abre la puerta con peso {doorIndex}");
    }
}