using System;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    private void Start()
    {
        EventManager.Instance.Register(GameEventTypes.OnCinematic, CursorDisabled);
        EventManager.Instance.Register(GameEventTypes.OnGameplay, CursorDisabled);
        EventManager.Instance.Register(GameEventTypes.OnPuzzle, CursorEnabled);
    }

    private void OnDestroy()
    {
        EventManager.Instance.Unregister(GameEventTypes.OnCinematic, CursorDisabled);
        EventManager.Instance.Unregister(GameEventTypes.OnGameplay, CursorDisabled);
        EventManager.Instance.Unregister(GameEventTypes.OnPuzzle, CursorEnabled);
    }

    void CursorEnabled(object sender, EventArgs e)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void CursorDisabled(object sender, EventArgs e)
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
