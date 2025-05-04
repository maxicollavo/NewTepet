using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public float shakeMagnitude = 0.3f;
    public float dampingSpeed = 1.0f;

    private Vector3 initialPosition;
    private float currentShakeDuration = 0f;

    void OnEnable()
    {
        initialPosition = transform.localPosition;
    }

    public void TriggerShake(float shakeDuration)
    {
        currentShakeDuration = shakeDuration;
    }

    void Update()
    {
        if (currentShakeDuration > 0)
        {
            transform.localPosition = initialPosition + Random.insideUnitSphere * shakeMagnitude;

            currentShakeDuration -= Time.deltaTime * dampingSpeed;
        }
        else
        {
            currentShakeDuration = 0f;
            transform.localPosition = initialPosition;
        }
    }
}
