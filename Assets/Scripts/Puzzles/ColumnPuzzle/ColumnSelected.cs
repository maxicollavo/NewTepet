using System;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class ColumnSelected : MonoBehaviour
{
    public Action<bool, ColumnSelected> OnSelectedAction;

    private bool isSelected;

    public GameObject columnToRotate;

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

        OnSelectedAction += interactManager.OnSelectedMethod;
    }



    private void OnMouseDown()
    {
        Debug.Log($"OnMouseDown en {gameObject.name}");
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
        Debug.Log("Entra al SelectedPiece");
        isSelected = true;
        Debug.Log($"is Selected es {isSelected}");
        EnableOutline();
        Debug.Log($"activa el outline");
        OnSelectedAction?.Invoke(isSelected, this);
        Debug.Log($"Se llama al Action");
    }

    public void DeselectPiece()
    {
        if (!isSelected) return;

        isSelected = false;
        DisableOutline();
        OnSelectedAction?.Invoke(isSelected, this);
    }
}