using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HieroglyficArmHandler : MonoBehaviour
{
    private Dictionary<Transform, Quaternion> originalRotations = new Dictionary<Transform, Quaternion>();

    public void EnableArm(Transform rightArm)
    {
        if (!originalRotations.ContainsKey(rightArm))
            originalRotations[rightArm] = rightArm.localRotation;

        rightArm.gameObject.SetActive(true);

        StartCoroutine(RotateArmToX(rightArm, 0f, 0.5f));
    }

    private Dictionary<Transform, Coroutine> disableCoroutines = new();

    public void DisableArm(Transform rightArm)
    {
        if (disableCoroutines.ContainsKey(rightArm)) return;

        if (originalRotations.TryGetValue(rightArm, out Quaternion original))
        {
            Coroutine c = StartCoroutine(RotateToRotation(rightArm, original, 0.2f));
            disableCoroutines[rightArm] = c;
        }
    }

    private IEnumerator RotateArmToX(Transform arm, float targetX, float duration)
    {
        Quaternion startRotation = arm.localRotation;

        Vector3 currentEuler = arm.localEulerAngles;
        Vector3 targetEuler = new Vector3(targetX, currentEuler.y, currentEuler.z);
        Quaternion targetRotation = Quaternion.Euler(targetEuler);

        float time = 0f;
        while (time < duration)
        {
            arm.localRotation = Quaternion.Lerp(startRotation, targetRotation, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        arm.localRotation = targetRotation;
    }

    private IEnumerator RotateToRotation(Transform arm, Quaternion targetRotation, float duration)
    {
        Vector3 startEuler = arm.localEulerAngles;
        Vector3 targetEuler = targetRotation.eulerAngles;

        float time = 0f;
        while (time < duration)
        {
            float t = time / duration;
            Vector3 newEuler = Vector3.Lerp(startEuler, targetEuler, t);
            arm.localEulerAngles = newEuler;
            time += Time.deltaTime;
            yield return null;
        }

        arm.localEulerAngles = targetEuler;

        arm.gameObject.SetActive(false);

        disableCoroutines.Remove(arm);
    }
}