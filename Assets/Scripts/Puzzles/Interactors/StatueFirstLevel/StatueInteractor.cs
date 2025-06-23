using System;
using UnityEngine;

public class StatueInteractor : MonoBehaviour, Interactor
{
    public Action<StatueInteractor> InteractorAction;

    Outline outline;
    BoxCollider coll;

    bool HasInteract;
    public bool isCat;

    private void Awake()
    {
        coll = GetComponent<BoxCollider>();
        outline = GetComponent<Outline>();
    }

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
        OnInteract();
    }

    void OnInteract()
    {
        if (isCat)
        {
            DisableOutline();
            InteractorAction?.Invoke(this);
            return;
        }

        if (HasInteract) return;

        DisableOutline();
        coll.enabled = false;
        InteractorAction?.Invoke(this);
    }
}
