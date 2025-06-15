using System;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class ColumnSelected : MonoBehaviour
{
    public Action<bool, ColumnSelected, Transform, Transform, Transform> OnSelectedAction;

    private bool isSelected;
    [HideInInspector] public bool hasWon;

    public GameObject columnToRotate;

    Outline outline;
    [HideInInspector] public BoxCollider coll;
    [SerializeField] ColumnInteractManager interactManager;
    [SerializeField] Transform columnTransform;

    public Transform forward;
    public Transform lookAtTarget;
    public bool isAligned;

    [Header("Interior Pieces")]
    public InteriorPieceSelector[] interiorPieces;

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
        if (isSelected) return;

        SelectedPiece();
    }

    private void OnMouseEnter()
    {
        if (isSelected) return;

        EnableOutline();
    }

    private void OnMouseExit()
    {
        if (isSelected) return;

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
        //Sonido de selección de columna (como el de las piezas del tablero)
        DisableOutline();
        OnSelectedAction?.Invoke(isSelected, this, columnTransform, forward, lookAtTarget);
    }

    public void DeselectPiece()
    {
        if (!isSelected) return;

        isSelected = false;
        DisableOutline();
        OnSelectedAction?.Invoke(isSelected, this, columnTransform, forward, lookAtTarget);
    }

    public void OnWin()
    {
        hasWon = true;
        DeselectPiece();
        coll.enabled = false;
    }
}