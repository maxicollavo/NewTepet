using System;
using System.Collections;
using UnityEngine;

public class OnScaleActions : MonoBehaviour, Interactor
{
    [SerializeField] Plate sidePlate;
    [SerializeField] ScaleManager manager;
    public Action<Plate, OnScaleActions> plateInteractAction;

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

    public IEnumerator CannotEnter()
    {
        EnableOutline();
        var originalColor = outline.OutlineColor;
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
        Debug.Log("Interactua y envia el Action");
        plateInteractAction?.Invoke(sidePlate, this);
    }
}
