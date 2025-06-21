using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverOnWin : MonoBehaviour
{
    [SerializeField] RiverPuzzleManager manager;
    [SerializeField] Animator anim;
    [SerializeField] ParticleSystem winParticles;

    private void Awake()
    {
        manager.OnWin += OnWinMethod;
    }

    private void OnWinMethod(RiverPuzzleManager manager)
    {
        anim.SetTrigger("Open");
        winParticles.Play();
    }
}
