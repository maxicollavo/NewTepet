using System;
using UnityEngine;

public class BoardPiece : MonoBehaviour
{
    //Va a llamar a un action y enviar la figura seleccionada y su waypoint al manager

    public Action<BoardPiece> OnPieceSelected;
    public BoardWaypoint currentWp;

    Outline outline;
    [HideInInspector]
    public BoxCollider coll;
    public bool IsSelected;
    public bool OnPositionWinner;

    private void Awake()
    {
        outline = GetComponent<Outline>();
        coll = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        outline.enabled = false;
        transform.position = currentWp.transform.position;
    }

    private void OnMouseDown()
    {
        SelectedPiece();
    }

    private void OnMouseEnter()
    {
        if (IsSelected) return;

        outline.enabled = true;
    }

    private void OnMouseExit()
    {
        if (IsSelected) return;

        outline.enabled = false;
    }

    public void SelectedPiece()
    {
        IsSelected = true;
        EnableOutline();
        OnPieceSelected?.Invoke(this);
    }

    public void DeselectPiece()
    {
        IsSelected = false;
        DisableOutline();
    }

    public void DisableOutline()
    {
        outline.enabled = false;
    }

    public void EnableOutline()
    {
        outline.enabled = true;
    }
}
