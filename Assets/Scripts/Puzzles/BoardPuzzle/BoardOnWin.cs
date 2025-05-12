using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardOnWin : MonoBehaviour
{
    [SerializeField] List<ParticleSystem> torchParticles;
    [SerializeField] List<BoxCollider> colliders;

    [SerializeField] BoardPuzzleManager manager;

    private void Awake()
    {
        manager.OnWin += OnWinMethod;
    }

    private void OnWinMethod(BoardPuzzleManager manager)
    {
        foreach (var particle in torchParticles)
        {
            particle.Play();
        }

        foreach (var coll in colliders)
        {
            coll.enabled = true;
        }
    }
}
