using System;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class ColumnSelected : MonoBehaviour
{
    public Action<bool, ColumnSelected> OnSelectedAction;

    private bool isSelected;

    Outline outline;
    [SerializeField] ColumnInteractManager interactManager;

    private void Awake()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;

        if (!interactManager.columnSelecteds.ContainsKey(this))
        {
            interactManager.columnSelecteds.Add(this, false);
        }
    }

    private void OnMouseDown()
    {
        if (isSelected) return;
        SelectedPiece();
    }

    private void OnMouseEnter()
    {
        if (isSelected) return;

        outline.enabled = true;
    }

    private void OnMouseExit()
    {
        if (isSelected) return;

        outline.enabled = false;
    }

    public void EnableOutline()
    {
        outline.enabled = true;
    }

    public void DisableOutline()
    {
        outline.enabled = false;
    }

    public void SelectedPiece()
    {
        isSelected = true;
        EnableOutline();
        OnSelectedAction?.Invoke(isSelected, this);
    }

    public void DeselectPiece()
    {
        isSelected = false;
        DisableOutline();
    }
}