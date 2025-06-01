using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class EnterColumnPuzzle : MonoBehaviour, Interactor
{
    [SerializeField] List<BoxCollider> columnColliders;
    [SerializeField] GameObject CM_PuzzleCamera;

    Outline outline;

    private void Start()
    {
        outline = GetComponent<Outline>();

        outline.enabled = false;
        EnableColumnColliders(false);
    }

    public void Interact()
    {
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
        }
        else
        {
            EventManager.Instance.Dispatch(GameEventTypes.OnGameplay, this, EventArgs.Empty);
        }
    }

    private void EnterPuzzleCamera(bool state)
    {
        CM_PuzzleCamera.SetActive(state);
    }

    private void EnableColumnColliders(bool state)
    {
        foreach (var collider in columnColliders)
        {
            collider.enabled = state;
        }
    }
}
