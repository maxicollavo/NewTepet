using System;
using UnityEngine;

public class NewEventManager : MonoBehaviour
{
    public static event Action<bool> OnPaused;
    public static event Action OnRead;
    public static event Action OnChangeRoom;

    public static void TriggerPause(bool state)
    {
        Debug.Log("TriggerPause called: " + state);
        OnPaused?.Invoke(state);
    }

    public static void TriggerRead()
    {
        OnRead?.Invoke();
    }

    public static void TriggerChangeRoom()
    {
        OnChangeRoom?.Invoke();
    }
}
