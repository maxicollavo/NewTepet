using System;
using UnityEngine;

public class OnScaleActions : MonoBehaviour, Interactor
{
    [SerializeField] Plate sidePlate;
    [SerializeField] ScaleManager manager;
    public Action<Plate> plateInteractAction;

    Outline outline;

    private void Awake()
    {
        outline = GetComponent<Outline>();

        manager.onScaleActions.Add(this);
    }

    private void Start()
    {
        outline.enabled = false;
    }

    public void Aiming()
    {
        EnableOutline();
    }

    public void DisableOutline()
    {
        outline.enabled = false;
        UIManager.Instance.ChangeCursor(false);
    }

    public void EnableOutline()
    {
        outline.enabled = false;
        UIManager.Instance.ChangeCursor(true);
    }

    public void Interact()
    {
        plateInteractAction?.Invoke(sidePlate);
    }
}
