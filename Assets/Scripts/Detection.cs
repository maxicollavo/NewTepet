using System;
using UnityEngine;

public class Detection : MonoBehaviour
{
    public float playerReach = 10f;

    float interactDistance = 5f;

    private bool onClick;

    private IRead lastReadeable = null;
    private Interactor lastInteractor = null;

    [SerializeField] private LayerMask ignoreMask;

    private bool onPause;

    private void OnEnable()
    {
        NewEventManager.OnPaused += TriggerPause;
    }

    private void OnDisable()
    {
        NewEventManager.OnPaused -= TriggerPause;
    }

    private void TriggerPause(bool state)
    {
        onPause = state;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            onClick = true;
        }

        Detect();

        onClick = false;
    }
    void Detect()
    {
        RaycastHit hit;
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        int layerMask = ~ignoreMask.value;

        IRead currentReadeable = null;
        Interactor currentInteractor = null;

        if (Physics.Raycast(ray, out hit, playerReach, layerMask))
        {
            if (hit.distance <= interactDistance)
            {
                if (hit.collider.TryGetComponent(out currentInteractor))
                {
                    currentInteractor.Aiming();

                    if (onClick)
                    {
                        if (onPause) return;

                        currentInteractor.Interact();
                    }
                }
            }

            if (hit.collider.TryGetComponent(out currentReadeable))
            {
                currentReadeable.Aiming();

                if (onClick)
                {
                    currentReadeable.Read();
                }
            }
        }

        if (lastInteractor != null && lastInteractor != currentInteractor)
        {
            lastInteractor.DisableOutline();
        }

        if (lastReadeable != null && lastReadeable != currentReadeable)
        {
            lastReadeable.DisableOutline();
        }

        lastReadeable = currentReadeable;
        lastInteractor = currentInteractor;
    }
}