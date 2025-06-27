using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatOnWin : MonoBehaviour
{
    [SerializeField] Animator boxAnim;
    [SerializeField] TrackerManager manager;
    [SerializeField] ParticleSystem WinParticleCat;
    [SerializeField] OwlManager owlManager;

    public bool HasWon;

    private void Awake()
    {
        manager.HieroglyphCompletedAction += OnWinMethod;
    }

    public void OnWinMethod()
    {
        if (HasWon) return;

        HasWon = true;
        boxAnim.SetTrigger("Open");
        WinParticleCat.Play();
        owlManager.DeactivateColliders();
    }

    private void OnWinMethod(TrackerManager manager)
    {
        OnWinMethod();
    }
}
