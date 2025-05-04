using System;
using UnityEngine;

public class HeadInteractor : MonoBehaviour, Interactor
{
    public Action<HeadInteractor> HeadAction;

    Outline outline;
    BoxCollider coll;

    private void Awake()
    {
        outline = GetComponent<Outline>();
        coll = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        outline.enabled = false;
        coll.enabled = false;
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
        DisableOutline();
        coll.enabled = false;
        HeadAction?.Invoke(this);
    }
}
