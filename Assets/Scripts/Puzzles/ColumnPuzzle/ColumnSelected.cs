using System;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class ColumnSelected : MonoBehaviour, Interactor
{
    public Action<bool, ColumnSelected> OnSelectedAction;

    private bool isSelected;

    Outline outline;
    [SerializeField] ColumnInteractManager interactManager;

    private void Awake()
    {
        outline = GetComponent<Outline>();

        if (!interactManager.columnSelecteds.ContainsKey(this))
        {
            interactManager.columnSelecteds.Add(this, false);
        }
    }

    public void Aiming()
    {
        if (isSelected) return;

        EnableOutline();
    }

    public void EnableOutline()
    {
        outline.enabled = true;
        UIManager.Instance.ChangeCursor(true);
    }

    public void DisableOutline()
    {
        outline.enabled = false;
        UIManager.Instance.ChangeCursor(false);
    }

    public void Interact()
    {
        if (isSelected) return;

        isSelected = true;
        OnSelectedAction?.Invoke(isSelected, this);
    }
}