using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OwlBoardOnWin : MonoBehaviour
{
    [SerializeField] BoardPuzzleManager manager;

    private void Awake()
    {
        manager.OnWin += OnWinMethod;
    }

    private void OnWinMethod(BoardPuzzleManager manager)
    {
    }
}
