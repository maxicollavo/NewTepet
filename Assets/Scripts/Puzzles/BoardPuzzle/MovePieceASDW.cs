using System;
using UnityEngine;

public class MovePieceASDW : MonoBehaviour, IButtonInput
{
    public Action<Vector2, MovePieceASDW> OnButtonPressed;

    public Outline outline => throw new NotImplementedException();

    private void Update()
    {
        if (!this.enabled) return;

        if (Input.GetKeyDown(KeyCode.W))
        {
            PressedButton(new Vector2(0,1));
        }
        else if (Input.GetKeyDown(KeyCode.A))
        {
            PressedButton(new Vector2(-1, 0));
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            PressedButton(new Vector2(0, -1));
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            PressedButton(new Vector2(1, 0));
        }
    }

    public void EnableOutline()
    {
        throw new NotImplementedException();
    }

    public void DisableOutline()
    {
        throw new NotImplementedException();
    }

    public void PressedButton(Vector2 dir)
    {
        OnButtonPressed?.Invoke(dir, this);
    }
}
