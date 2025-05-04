using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public static EventManager Instance { get; private set; }

    Dictionary<GameEventTypes, EventHandler> events = new Dictionary<GameEventTypes, EventHandler>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void Register(GameEventTypes eventName, EventHandler handler)
    {
        if (!events.ContainsKey(eventName))
        {
            events.Add(eventName, null);
        }
        events[eventName] += handler;
    }

    public void Unregister(GameEventTypes eventName, EventHandler handler)
    {
        if (events.ContainsKey(eventName))
        {
            events[eventName] -= handler;
        }
    }

    public void Dispatch(GameEventTypes eventName, object sender, EventArgs args)
    {
        if (events.ContainsKey(eventName) && events[eventName] != null)
        {
            events[eventName].Invoke(sender, args);
        }
    }
}

public enum GameEventTypes
{
    OnGameplay,
    OnCinematic,
    OnPuzzle,
    OnPickeable
}