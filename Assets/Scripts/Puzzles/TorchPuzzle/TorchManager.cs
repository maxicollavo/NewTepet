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
        foreach (var t in torches)
        {
            bool shouldBeDown = correctTorchesIndex.Contains(t.index);

            if (shouldBeDown != t.IsUpsideDown)
            {
                Debug.Log("Puzzle incorrecto");
                return;
            }
        }

        if (!HasWon)
        {
            HasWon = true;
        }
    }

    private void OnWinMethod(Torch torch)
    {
        if (!HasWon) return;

        doorCeilingCollider.SetActive(true);
        shake.TriggerShake();
        doorAnim.SetTrigger("Open");
    }
}