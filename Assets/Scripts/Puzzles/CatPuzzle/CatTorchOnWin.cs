using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatTorchOnWin : MonoBehaviour
{
    [SerializeField] TorchManager manager;

    [SerializeField] GameObject lights;
    [SerializeField] BoxCollider coll;

    private void OnEnable()
    {
        manager.TorchOnWinAction += OnWinMethod;
    }

    private void OnDisable()
    {
        manager.TorchOnWinAction -= OnWinMethod;
    }

    private void OnWinMethod(TorchManager manager)
    {
        lights.SetActive(true);
        coll.enabled = true;
    }
}