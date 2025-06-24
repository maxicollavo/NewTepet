using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatTorchOnWin : MonoBehaviour
{
    [SerializeField] TorchManager manager;

    [SerializeField] GameObject lights;
    [SerializeField] GameObject path;
    [SerializeField] GameObject[] colliders;

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
        path.SetActive(true);
        foreach (var collider in colliders)
            collider.SetActive(true);
    }
}