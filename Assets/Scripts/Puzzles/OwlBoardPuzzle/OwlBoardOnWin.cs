using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OwlBoardOnWin : MonoBehaviour
{
    [SerializeField] BoardPuzzleManager manager;
    [SerializeField] Animator anim;
    [SerializeField] ParticleSystem OnBoardWin;

    private void Awake()
    {
        manager.OnWin += OnWinMethod;
    }

    private void OnWinMethod(BoardPuzzleManager manager)
    {
        anim.SetTrigger("Open");
        OnBoardWin.Play();
    }
}
