using UnityEngine;
using Unity.Cinemachine;
using System;

public class CameraShake : MonoBehaviour
{
    private float shakeMagnitude = 0.3f;
    private CinemachineCameraOffset cameraOffset;
    private float shakeTime;
    private Vector3 originalOffset;
    private bool initialized = false;

    void Awake()
    {
        TryInit();

        EventManager.Instance.Register(GameEventTypes.OnDoorOpen, OnTriggerShake);

    }

    private void OnDestroy()
    {
        EventManager.Instance.Unregister(GameEventTypes.OnDoorOpen, OnTriggerShake);

    }

    private void TryInit()
    {
        if (!initialized)
        {
            cameraOffset = GetComponent<CinemachineCameraOffset>();
            if (cameraOffset != null)
            {
                originalOffset = cameraOffset.Offset;
                initialized = true;
            }
        }
    }

    private void OnTriggerShake(object sender, EventArgs e)
    {
        TriggerShake(1f, 0.02f);
    }

    public void TriggerShake(float duration, float magnitude)
    {
        TryInit();
        if (!initialized) return;

        shakeMagnitude = magnitude;
        shakeTime = duration;
    }

    void Update()
    {
        if (!initialized) return;

        if (shakeTime > 0)
        {
            cameraOffset.Offset = originalOffset + UnityEngine.Random.insideUnitSphere * shakeMagnitude;
            shakeTime -= Time.deltaTime;
        }
        else
        {
            cameraOffset.Offset = originalOffset;
        }
    }
}
