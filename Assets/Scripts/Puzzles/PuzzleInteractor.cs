using System;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class PuzzleInteractor : MonoBehaviour, Interactor
{
    public Action<PuzzleInteractor> PuzzleAction;
    [HideInInspector]
    public Outline outline;

    private void Start()
    {
        if (outline == null) outline = GetComponent<Outline>();

        outline.enabled = false;
    }

    public void Interact()
    {
        PuzzleMethod();
    }

    public void PuzzleMethod()
    {
        PuzzleAction?.Invoke(this);
    }

    public void DisableOutline()
    {
        outline.enabled = false;

        UIManager.Instance.ChangeCursor(false);
    }

    public void EnableOutline()
    {
        outline.enabled = true;
    }

    public void Aiming()
    {
        EnableOutline();

        UIManager.Instance.ChangeCursor(true);
    }
}
