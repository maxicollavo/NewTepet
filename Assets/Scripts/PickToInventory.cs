using System;
using UnityEngine;

public class PickToInventory : MonoBehaviour, Interactor
{
    Outline outline;
    [SerializeField] GameObject handObj;
    [SerializeField] PuzzleInteractor interactor;

    private void Awake()
    {
        outline = GetComponent<Outline>();
    }

    private void Start()
    {
        outline.enabled = false;
    }

    public void Interact()
    {
        DisableOutline();
        handObj.SetActive(true);
        gameObject.SetActive(false);
        GameManager.Instance.HasPiece = true;
    }

    public void DisableOutline()
    {
        outline.enabled = false;

        UIManager.Instance.ChangeCursor(false);
    }

    void EnableOutline()
    {
        outline.enabled = true;
    }

    public void Aiming()
    {
        EnableOutline();

        UIManager.Instance.ChangeCursor(true);
    }
}
