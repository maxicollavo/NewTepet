using System;
using UnityEngine;

public class BoardButton : MonoBehaviour, IOutlineButton
{
    public Action<Vector2, BoardButton> OnButtonPressed;
    [SerializeField] private Vector2 direction;

    private Outline _outline;

    public Outline outline => _outline;

    private void Awake()
    {
        _outline = GetComponent<Outline>();
    }

    private void Start()
    {
        _outline.enabled = false;
    }

    private void OnMouseDown()
    {
        PressedButton();
    }

    public void PressedButton()
    {
        OnButtonPressed?.Invoke(direction, this);
    }

    public void DisableOutline()
    {
        _outline.enabled = false;
    }

    public void EnableOutline()
    {
        _outline.enabled = true;
    }

    public void PressedButton(Vector2 dir)
    {
        throw new NotImplementedException();
    }
}
