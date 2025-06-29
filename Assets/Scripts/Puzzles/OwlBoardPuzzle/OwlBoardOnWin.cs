using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OwlBoardOnWin : MonoBehaviour
{
    [SerializeField] BoardPuzzleManager manager;
    [SerializeField] Animator anim;
    [SerializeField] ParticleSystem OnBoardWin;
    [SerializeField] BoxCollider owlColl;
    [SerializeField] AudioSource WinBoxSound;
    [SerializeField] AudioSource WinSound;

    private void Awake()
    {
        manager.OnWin += OnWinMethod;
    }

    private void OnWinMethod(BoardPuzzleManager manager)
    {
        anim.SetTrigger("Open");
        OnBoardWin.Play();
        WinBoxSound.Play();
        WinSound.Play();
        owlColl.enabled = false;
    }
}
