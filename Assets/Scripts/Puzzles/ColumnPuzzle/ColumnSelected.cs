using System;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class ColumnSelected : MonoBehaviour
{
    public Action<bool, ColumnSelected> OnSelectedAction;

    private bool isSelected;

    Outline outline;
    [HideInInspector] public BoxCollider coll;
    [SerializeField] ColumnInteractManager interactManager;

    [Header("Interior Pieces")]
    public InteriorPieceSelector[] interiorPieces;

    private void Awake()
    {
        outline = GetComponent<Outline>();
        coll = GetComponent<BoxCollider>();
        outline.enabled = false;

        OnSelectedAction += interactManager.OnSelectedMethod;
    }

    private void OnMouseDown()
    {
        if (isSelected || interactManager.hasWon) return;

        SelectedPiece();
    }

    private void OnMouseEnter()
    {
        if (isSelected || interactManager.hasWon) return;

        EnableOutline();
    }

    private void OnMouseExit()
    {
        if (isSelected || interactManager.hasWon) return;

        DisableOutline();
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
        AudioManager.Instance.PlaySound("SelectPiece");
        DisableOutline();
        OnSelectedAction?.Invoke(isSelected, this);
    }

    public void DeselectPiece()
    {
        Debug.Log("Entra a Deselect Piece");
        if (!isSelected) return;
        Debug.Log("Entra a Deselect Piece y pasa el primer if");
        isSelected = false;
        DisableOutline();
        OnSelectedAction?.Invoke(isSelected, this);
        if (interiorPieces.Length > 0)
        {
            foreach (var piece in interiorPieces)
            {
                piece.DisableOutline();
            }
        }
    }
}