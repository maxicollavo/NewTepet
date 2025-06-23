using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatOnWin : MonoBehaviour
{
    [SerializeField] Animator boxAnim;
    [SerializeField] TrackerManager manager;

    private void Awake()
    {
        manager.JeroglificAction += OnWinMethod;
    }

    private void OnWinMethod(TrackerManager manager)
    {
        boxAnim.SetTrigger("Open");
    }
}
