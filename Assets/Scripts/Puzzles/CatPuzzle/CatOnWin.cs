using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatOnWin : MonoBehaviour
{
    [SerializeField] Animator boxAnim;
    [SerializeField] TrackerManager manager;
    [SerializeField] ParticleSystem WinParticleCat;
    [SerializeField] BoxCollider[] colliders;

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

        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }
    }

    private void OnWinMethod(TrackerManager manager)
    {
        OnWinMethod();
    }
}
