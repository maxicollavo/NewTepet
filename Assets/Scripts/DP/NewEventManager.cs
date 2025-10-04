using System;
using UnityEngine;

public class NewEventManager : MonoBehaviour
{
    public static event Action<bool> OnPaused;

    public static void TriggerPause(bool state)
    {
        Debug.Log("TriggerPause called: " + state);
        OnPaused?.Invoke(state);
    }
}
