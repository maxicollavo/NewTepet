using System;
using System.Collections.Generic;
using UnityEngine;

public class TorchManager : MonoBehaviour
{
    [SerializeField] List<Torch> torches;
    [SerializeField] List<int> correctTorchesIndex;
    [SerializeField] CameraShake shake;
    [SerializeField] GameObject doorCeilingCollider;
    int counter;
    bool HasWon;
    bool canEnter = true;

    [SerializeField] Animator doorAnim;

    private void Start()
    {
        foreach (var t in torches) t.OnInteractAction += OnInteract;
        foreach (var t in torches) t.OnAnimFinishAction += OnWinMethod;
    }

    private void OnInteract(Torch torch, int index)
    {
        CheckPuzzleState(torch);
    }

    private void CheckPuzzleState(Torch torch)
    {
        Debug.Log(HasWon);
        foreach (var t in torches)
        {
            bool shouldBeDown = correctTorchesIndex.Contains(t.index);

            if (shouldBeDown != t.IsUpsideDown)
            {
                Debug.Log("Puzzle incorrecto");
                return;
            }
        }

        HasWon = true;
        Debug.Log(HasWon);
    }

    private void OnWinMethod(Torch torch)
    {
        if (!HasWon) return;

        if (canEnter)
        {
            shake.TriggerShake();
            doorAnim.SetTrigger("Open");
            canEnter = false;
        }
    }
}