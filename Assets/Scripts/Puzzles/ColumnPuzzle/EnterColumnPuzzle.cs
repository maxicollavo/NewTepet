using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class EnterColumnPuzzle : MonoBehaviour, Interactor
{
    [SerializeField] List<ColumnSelected> columns;
    [SerializeField] GameObject CM_PuzzleCamera;
    [SerializeField] GameObject ColumnUI;
    [SerializeField] ColumnInteractManager columnInteractManager;
    Outline outline;

    [HideInInspector] public bool canInteract;

    private void Start()
    {
        outline = GetComponent<Outline>();
        canInteract = true;
        outline.enabled = false;
        EnableColumnColliders(false);
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
    }

    public void Aiming()
    {
        if (!canInteract) return;

        EnableOutline();
        UIManager.Instance.ChangeCursor(true);
    }

    public void EnterPuzzle(bool state)
    {
        
        EnterPuzzleCamera(state);
        EnableColumnColliders(state);

        if (state)
        {
            EventManager.Instance.Dispatch(GameEventTypes.OnPuzzle, this, EventArgs.Empty);
            columnInteractManager.canRotate = true;
            ColumnUI.SetActive(true);
        }
        else
        {
            EventManager.Instance.Dispatch(GameEventTypes.OnGameplay, this, EventArgs.Empty);
            columnInteractManager.canRotate = false;

            columnInteractManager.ClearSelection();
            ColumnUI.SetActive(false);
        }
    }

    private void EnterPuzzleCamera(bool state)
    {
        CM_PuzzleCamera.SetActive(state);
    }

    private void EnableColumnColliders(bool state)
    {
        foreach (var column in columns)
        {
            if (columnInteractManager.hasWon) return;

            column.coll.enabled = state;
        }
    }
}
