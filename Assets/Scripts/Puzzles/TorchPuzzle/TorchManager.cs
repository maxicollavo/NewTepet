using System.Collections.Generic;
using UnityEngine;

public class TorchManager : MonoBehaviour
{
    [SerializeField] List<Torch> torches;
    [SerializeField] List<int> correctTorchesIndex;

    int counter;
    bool HasWon;

    [SerializeField] Animator doorAnim;

    private void Start()
    {
        foreach (var t in torches) t.TorchAction += OnInteract;
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
            OnWin();
        }
    }

    private void OnWin()
    {
        doorAnim.SetTrigger("Open");
    }
}