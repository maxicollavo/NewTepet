using System.Collections;
using UnityEngine;

public class Ring : MonoBehaviour
{
    public int ringIndex;
    public bool isSelected { private get; set; }
    Outline outline;

    private void Awake()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;
    }

    public void OnStartRotation()
    {
        StartCoroutine(WaitRotation());
    }

    private IEnumerator WaitRotation()
    {
        DisableOutline();

        while (RingPuzzleManager.Instance.isRotating)
        {
            yield return null;
        }

        if (isSelected)
        {
            EnableOutline();
        }
    }

    private void OnMouseDown()
    {
        if (!RingPuzzleManager.Instance.canInteract) return;

        RingPuzzleManager.Instance.SelectRing(ringIndex);
    }

    private void OnMouseEnter()
    {
        if (!RingPuzzleManager.Instance.canInteract || isSelected) return;
        EnableOutline();
    }

    private void OnMouseExit()
    {
        if (!RingPuzzleManager.Instance.canInteract || isSelected) return;

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
