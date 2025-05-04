using System;
using UnityEngine;

public class InteractJeroglific : MonoBehaviour, Interactor
{
    [SerializeField] Outline outline;

    [SerializeField] TrailManager manager;

    private void Start()
    {
        outline.enabled = false;
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

    public void DisableOutline()
    {
        outline.enabled = false;

        UIManager.Instance.ChangeCursor(false);
    }

    public void Interact()
    {
        DisableOutline();
        //manager.OnJeroglific = true;
        //manager.EnterToJeroglific();
    }
}
