using System;
using UnityEngine;

public class NewEventManager : MonoBehaviour
{
    public static event Action<bool> OnPaused;
    public static event Action OnFreezePlayer;
    public static event Action OnUnfreezePlayer;
    public static event Action OnChangeRoom;

    public static void TriggerPause(bool state)
    {
        OnPaused?.Invoke(state);
    }

    public static void TriggerFreeze(bool state)
    {
        if (state)
            OnFreezePlayer?.Invoke();
        else
            OnUnfreezePlayer?.Invoke();

    }

    public static void TriggerChangeRoom()
    {
        OnChangeRoom?.Invoke();
    }
}
