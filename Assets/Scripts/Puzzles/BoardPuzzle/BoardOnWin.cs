using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardOnWin : MonoBehaviour
{
    [SerializeField] GameObject torchParticles;
    [SerializeField] List<BoxCollider> colliders;
    [SerializeField] List<Torch> torches;
    [SerializeField] List<BoxCollider> boardColliders;
    [SerializeField] List<GameObject> lights;

    [SerializeField] BoardPuzzleManager manager;

    private void Awake()
    {
        manager.OnWin += OnWinMethod;
    }

    private void OnWinMethod(BoardPuzzleManager manager)
    {
        torchParticles.SetActive(true);

        foreach (var coll in colliders)
        {
            coll.enabled = true;
        }

        foreach (var light in lights)
        {
            light.SetActive(true);
        }

        foreach (var colls in boardColliders)
        {
            colls.enabled = false;
        }

        foreach(var torch in torches)
        {
            torch.FireSound.Play();
        }
    }
}
