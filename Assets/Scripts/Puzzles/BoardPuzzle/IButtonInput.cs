using System;
using UnityEngine;

public interface IButtonInput
{
    void PressedButton(Vector2 dir);

}

public interface IOutlineButton : IButtonInput
{
    Outline outline { get; }
    void EnableOutline();
    void DisableOutline();
}