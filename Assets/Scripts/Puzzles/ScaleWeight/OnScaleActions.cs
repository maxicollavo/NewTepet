using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class OnScaleActions : MonoBehaviour, Interactor
{
    [SerializeField] Plate sidePlate;
    [SerializeField] ScaleManager manager;
    public Action<Plate, OnScaleActions> plateInteractAction;
    public Action<OnScaleActions> garbageInteractAction;

    public bool isScale;
    Outline outline;
    Color originalColor;

    private void Awake()
    {
        outline = GetComponent<Outline>();

        manager.onScaleActions.Add(this);
    }

    private void Start()
    {
        outline.enabled = false;
        originalColor = outline.OutlineColor;
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

    public IEnumerator CannotEnter()
    {
        EnableOutline();
        outline.OutlineColor = Color.red;
        yield return new WaitForSeconds(0.5f);
        outline.OutlineColor = originalColor;
        DisableOutline();
    }

    public void EnableOutline()
    {
        outline.enabled = true;
        UIManager.Instance.ChangeCursor(true);
    }

    public void Interact()
    {
        if (isScale)
            plateInteractAction?.Invoke(sidePlate, this);
        else
            garbageInteractAction?.Invoke(this);
    }
}
