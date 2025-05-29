using System;
using System.Collections.Generic;
using UnityEngine;

public class TorchManager : MonoBehaviour
{
    [SerializeField] List<Torch> torches;
    [SerializeField] List<int> correctTorchesIndex;
    [SerializeField] CameraShake shake;
    bool HasWon;
    bool canEnter = true;

    [SerializeField] MimicInteraction mimic;

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
                return;
            }
        }

        HasWon = true;
    }

    private void OnWinMethod(Torch torch)
    {
        if (!HasWon) return;

        if (canEnter)
        {
            shake.TriggerShake();
            mimic.canInteract = true;
            mimic.SetSphereMaterials();
            canEnter = false;
        }
    }
}