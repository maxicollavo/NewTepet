using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class EnterColumnPuzzle : MonoBehaviour, Interactor
{
    [SerializeField] List<BoxCollider> columnColliders;
    [SerializeField] GameObject CM_PuzzleCamera;
    public Outline outline;

    private void Start()
    {
        outline = GetComponent<Outline>();

        outline.enabled = false;
    }

    public void Interact()
    {
        TurnPuzzleCamera(true);

        foreach (BoxCollider collider in columnColliders)
        {
            collider.enabled = true;
        }
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

    public void TurnPuzzleCamera(bool state)
    {
        if (state)
        {
            CM_PuzzleCamera.SetActive(true);
        }
        else
        {
            CM_PuzzleCamera.SetActive(false);
        }
    }
}
