using System.Collections.Generic;
using System;
using UnityEngine;

public class LockInteractor : MonoBehaviour, Interactor
{
    [SerializeField] GameObject playerCam;
    [SerializeField] GameObject lockCam;

    [HideInInspector]
    public bool OnLock;

    [SerializeField] Outline outline;

    private void Start()
    {
        outline.enabled = false;
    }

    public void LockDisabled()
    {
        EventManager.Instance.Dispatch(GameEventTypes.OnGameplay, this, EventArgs.Empty);
        OnLock = false;
        playerCam.SetActive(true);
        lockCam.SetActive(false);
    }

    public void Interact()
    {
        EventManager.Instance.Dispatch(GameEventTypes.OnPuzzle, this, EventArgs.Empty);
        DisableOutline();
        OnLock = true;
        lockCam.SetActive(true);
        playerCam.SetActive(false);
    }

    public void DisableOutline()
    {
        outline.enabled = false;
    }

    void EnableOutline()
    {
        outline.enabled = true;
    }

    public void Aiming()
    {
        EnableOutline();
    }
}