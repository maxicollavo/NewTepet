using System;
using System.Collections.Generic;
using UnityEngine;

public class TorchManager : MonoBehaviour
{
    public Action<TorchManager> TorchOnWinAction;

    [SerializeField] List<Torch> torches;
    [SerializeField] List<int> correctTorchesIndex;
    [SerializeField] CameraShake shake;
    bool HasWon;
    bool canEnter = true;
    [SerializeField] bool onSecondLevel;

    [SerializeField] GameObject mimicSphere;
    Animator mimicSphereAnim;
    MeshRenderer mimicSphereRend;
    Material mimicSphereFillMat;

    private void OnEnable()
    {
        if (mimicSphere != null)
        {
            mimicSphereAnim = mimicSphere.transform.parent.GetComponent<Animator>();
            mimicSphereRend = mimicSphere.GetComponent<MeshRenderer>();
            mimicSphereFillMat = mimicSphereRend.materials[1];
        }
    }

    private void Start()
    {
        foreach (var t in torches) t.OnInteractAction += OnInteract;
        foreach (var t in torches) t.OnAnimFinishAction += OnWinMethod;
    }

    private void OnDisable()
    {
        foreach (var t in torches) t.OnInteractAction -= OnInteract;
        foreach (var t in torches) t.OnAnimFinishAction -= OnWinMethod;
    }

    private void OnInteract(Torch torch, int index)
    {
        CheckPuzzleState(torch);
    }

    private void CheckPuzzleState(Torch torch)
    {
        foreach (var t in torches)
        {
            bool shouldBeDown = correctTorchesIndex.Contains(t.index);

            if (shouldBeDown != t.IsUpsideDown)
            {
                return;
            }
        }

        HasWon = true;
    }

    private void OnWinMethod(Torch torch)
    {
        if (!HasWon) return;

        if (onSecondLevel)
        {
            TorchOnWinAction?.Invoke(this);
            return;
        }

        if (canEnter)
        {
            shake.TriggerShake();
            mimicSphereAnim.SetBool("CanStart", true);
            mimicSphereFillMat.SetFloat("_OffsetPoint", 1f);
            canEnter = false;
        }
    }
}