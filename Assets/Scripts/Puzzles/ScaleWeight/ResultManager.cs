using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ResultManager : MonoBehaviour
{
    [SerializeField] WeightManager weightManager;
    [SerializeField] Animator[] doorAnims;
    [SerializeField] AudioSource[] doorSounds3D;
    [SerializeField] BoxCollider[] platesColl;
    [SerializeField] MeshCollider[] garbageColl;
    [SerializeField] VisualEffect visualEffect;
    bool[] doorsDone;
    int counter;

    private void Awake()
    {
        weightManager.OnResultAction += OnResultMethod;
        doorsDone = new bool[doorAnims.Length];
    }

    private void OnResultMethod(float result, bool canOpenDoor)
    {
        int rounded = Mathf.RoundToInt(result);
        switch (rounded)
        {
            case 2:
                TryOpenDoor(0);
                break;
            case 7:
                TryOpenDoor(1);
                Debug.Log("Llegue a 7");
                break;
            case 9:
                TryOpenDoor(2);
                break;
            case 28:
                TryOpenDoor(3);
                break;
            case 50:
                TryOpenDoor(4);
                break;
            default:
                Debug.Log("No se llega a ningun peso");
                break;
        }

        if (canOpenDoor)
        {
            TryOpenDoor(4);
        }
    }

    private void TryOpenDoor(int doorIndex)
    {
        if (doorIndex >= doorAnims.Length)
        {
            return;
        }

        if (doorsDone[doorIndex]) return;
        doorAnims[doorIndex].SetTrigger("Open");
        doorSounds3D[doorIndex].Play();
        doorsDone[doorIndex] = true;
        Debug.Log("se abre la ultima puerta");

        if (doorIndex == 4)
        {
            visualEffect.Stop();
            //visualEffect.SetFloat("alpha", 0f);
            ObjectCreator.Instance.canPick = false;

            foreach (var plate in platesColl)
            {
                plate.enabled = false;
            }

            foreach (var garb in garbageColl)
            {
                garb.enabled = false;
            }
        }
    }
}