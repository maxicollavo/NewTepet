using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class EnterRiverPuzzle : MonoBehaviour, Interactor
{
    [SerializeField] GameObject CM_PuzzleCamera;
    [SerializeField] GameObject RiverUI;
    //[SerializeField] GameObject pieces;
    Outline outline;

    [HideInInspector] public bool canInteract;

    private void Start()
    {
        outline = GetComponent<Outline>();
        canInteract = true;
        outline.enabled = false;
        //EnableColumnColliders(false);
    }

    public void Interact()
    {
        if (!canInteract) return;

        EnterPuzzle(true);
    }

    public void DisableOutline()
    {
        outline.enabled = false;
        UIManager.Instance.ChangeCursor(false);
    }

    public void EnableOutline()
    {
        outline.enabled = true;
        UIManager.Instance.ChangeCursor(true);
    }

    public void Aiming()
    {
        if (!canInteract) return;

        EnableOutline();
    }

    public void EnterPuzzle(bool state)
    {
        EnterPuzzleCamera(state);
        //EnableColumnColliders(state);

        if (state)
        {
            EventManager.Instance.Dispatch(GameEventTypes.OnPuzzle, this, EventArgs.Empty);
            //columnInteractManager.canRotate = true; Aca tendria que poner can move pieces en true
            RiverUI.SetActive(true);
            DisableOutline();
            canInteract = false;
        }
        else
        {
            EventManager.Instance.Dispatch(GameEventTypes.OnGameplay, this, EventArgs.Empty);
            //columnInteractManager.canRotate = false; Aca pongo move pieces en false
            //columnInteractManager.ClearSelection(); //Sacamos el Outline?
            RiverUI.SetActive(false);
            canInteract = true;
        }
    }

    private void EnterPuzzleCamera(bool state)
    {
        CM_PuzzleCamera.SetActive(state);
    }

    private void EnableColumnColliders(bool state)
    {
        //foreach (var piece in pieces)
        //{
        //    //if (columnInteractManager.hasWon) return; Si se gano retornamos
        //    piece.coll.enabled = state;
        //}
    }
}
