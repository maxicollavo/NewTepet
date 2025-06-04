using System;
using UnityEngine;

public class PyramidPicking : MonoBehaviour, Interactor
{
    public Action<PyramidPicking> OnPicking;

    [SerializeField] Outline outline;

    private void Start()
    {
        outline.enabled = false;
    }

    public void DisableOutline()
    {
        outline.enabled = false;

        UIManager.Instance.ChangeCursor(false);
    }

    void EnableOutline()
    {
        outline.enabled = true;
    }

    public void Aiming()
    {
        EnableOutline();

        UIManager.Instance.ChangeCursor(true);
    }

    private void GrabPyramid()
    {
        OnPicking?.Invoke(this);
        UIManager.Instance.ChangeCursor(false);
    }

    public void Interact()
    {
        GrabPyramid();
    }
}