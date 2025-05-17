using UnityEngine;
using Unity.Cinemachine;

public class CameraShake : MonoBehaviour
{
    private float shakeDuration = 0.75f;
    private float shakeMagnitude = 0.3f;

    private CinemachineCameraOffset cameraOffset;
    private float shakeTime;
    private Vector3 originalOffset;

    void Awake()
    {
        cameraOffset = GetComponent<CinemachineCameraOffset>();
        originalOffset = cameraOffset.Offset;
    }

    public void TriggerShake()
    {
        shakeTime = shakeDuration;
    }

    void Update()
    {
        if (shakeTime > 0)
        {
            cameraOffset.Offset = originalOffset + Random.insideUnitSphere * shakeMagnitude;
            shakeTime -= Time.deltaTime;
        }
        else
        {
            cameraOffset.Offset = originalOffset;
        }
    }
}
