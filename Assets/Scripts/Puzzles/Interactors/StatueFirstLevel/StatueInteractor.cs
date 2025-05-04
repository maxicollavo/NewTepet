using System;
using UnityEngine;

public class StatueInteractor : MonoBehaviour, Interactor
{
    public Action<StatueInteractor> InteractorAction;

    Outline outline;
    BoxCollider coll;

    bool HasInteract;

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
        if (HasInteract) return;
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
        if (HasInteract) return;

        HasInteract = true;
        DisableOutline();
        coll.isTrigger = false;
        InteractorAction?.Invoke(this);
    }
}
