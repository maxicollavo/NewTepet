using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ring : MonoBehaviour
{
    [SerializeField] private float rotationDuration = 0.25f;

    private bool isRotating;

    private float currentYRotation;

    private IEnumerator RotateRing()
    {
        isRotating = true;

        Quaternion startRotation = transform.localRotation;

        Quaternion targetRotation =
            Quaternion.Euler(
                transform.localEulerAngles.x,
                currentYRotation,
                transform.localEulerAngles.z
            );

        float time = 0f;

        while (time < rotationDuration)
        {
            time += Time.deltaTime;

            transform.localRotation = Quaternion.Lerp(
                startRotation,
                targetRotation,
                time / rotationDuration
            );

            yield return null;
        }

        transform.localRotation = targetRotation;

        isRotating = false;
    }
}
