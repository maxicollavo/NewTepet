using System;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class ColumnSelected : MonoBehaviour
{
    public Action<bool, ColumnSelected> OnSelectedAction;

    private bool isSelected;
    public bool isLeft;

    [HideInInspector] public bool hasWon;

    public GameObject columnToRotate;

    Outline outline;
    [HideInInspector] public BoxCollider coll;
    [SerializeField] ColumnInteractManager interactManager;

    private void Awake()
    {
        outline = GetComponent<Outline>();
        coll = GetComponent<BoxCollider>();
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
        isSelected = true;
        EnableOutline();
        OnSelectedAction?.Invoke(isSelected, this);
    }

    public void DeselectPiece()
    {
        if (!isSelected) return;

        isSelected = false;
        DisableOutline();
        OnSelectedAction?.Invoke(isSelected, this);
    }

    public void OnWin()
    {
        hasWon = true;
        DeselectPiece();
        coll.enabled = false;
    }
}