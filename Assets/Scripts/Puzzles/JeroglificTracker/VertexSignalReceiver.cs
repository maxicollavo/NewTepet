using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexSignalReceiver : MonoBehaviour
{
    public Transform rightArm;

    private Dictionary<Transform, Quaternion> originalRotations = new Dictionary<Transform, Quaternion>();

    private void Awake()
    {
        originalRotations[rightArm] = rightArm.localRotation;
    }

    public void EnableArm()
    {
        Debug.Log("Enable arm");
        StartCoroutine(RotateArmToX(rightArm, 0f, 0.25f));
    }

    public void DisableArm()
    {
        Quaternion original = originalRotations[rightArm];
        StartCoroutine(RotateToRotation(rightArm, original, 0.25f));
    }

    private IEnumerator RotateArmToX(Transform arm, float targetX, float duration)
    {
        Vector3 startEuler = arm.localEulerAngles;
        Vector3 targetEuler = new Vector3(targetX, startEuler.y, startEuler.z);

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
        Debug.Log("Termino la corrutina");
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
    }
}
