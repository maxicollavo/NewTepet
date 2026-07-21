using System.Collections;
using UnityEngine;

public class Ring : MonoBehaviour
{
    public int ringIndex;
    public bool isSelected { private get; set; }

    private Outline outline;

    private Color hoverColor;
    private Color selectedColor;

    private void Awake()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;

        ColorUtility.TryParseHtmlString("#DD9518", out hoverColor);
        ColorUtility.TryParseHtmlString("#FF0003", out selectedColor);
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
            EnableSelectedOutline();
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

        EnableHoverOutline();
        UIManager.Instance.ChangeCursor(true);
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

    public void EnableHoverOutline()
    {
        ChangeOutlineColor(hoverColor);
        outline.enabled = true;
    }

    public void EnableSelectedOutline()
    {
        ChangeOutlineColor(selectedColor);
        outline.enabled = true;
    }

    private void ChangeOutlineColor(Color color)
    {
        outline.OutlineColor = color;
    }
}