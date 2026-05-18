using System;
using System.Collections;
using UnityEngine;

public class Ring : MonoBehaviour
{
    public int ringIndex;

    Outline outline;

    private void Awake()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;
    }

    private void OnMouseDown()
    {
        if (!RingPuzzleManager.Instance.canInteract) return;

        RingPuzzleManager.Instance.SelectRing(ringIndex);
    }

    private void OnMouseEnter()
    {
        if (!RingPuzzleManager.Instance.canInteract) return;

        EnableOutline();
    }

    private void OnMouseExit()
    {
        if (!RingPuzzleManager.Instance.canInteract) return;

        DisableOutline();
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
}
