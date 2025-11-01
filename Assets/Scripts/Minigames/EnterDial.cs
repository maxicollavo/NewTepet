using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterDial : MonoBehaviour
{
    public Action EnterDialAction;
    [SerializeField] PuzzleInteractor _puzzleInteractor;

    private void Start()
    {
        _puzzleInteractor.PuzzleAction += EnterDialMethod;
    }
    private void EnterDialMethod(PuzzleInteractor interactor)
    {
        EnterDialAction?.Invoke();
    }
}
