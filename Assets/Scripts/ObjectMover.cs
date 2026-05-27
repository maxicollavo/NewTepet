using System;
using System.Collections;
using UnityEngine;

public class ObjectMover : MonoBehaviour
{
    [SerializeField] private float minDuration = 0.2f;
    [SerializeField] private float maxDuration = 0.4f;

    [SerializeField] private float arcMultiplier = 0.3f;
    [SerializeField] private float minArcHeight = 0.15f;
    [SerializeField] private float maxArcHeight = 0.5f;

    private bool isMoving;

    public bool IsMoving => isMoving;

    public static ObjectMover Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void MoveToSlot(Transform obj, Transform target, Action onComplete = null)
    {
        if (isMoving) return;

        StartCoroutine(MoveToSlotCoroutine(obj, target, onComplete));
    }

    private IEnumerator MoveToSlotCoroutine(Transform obj, Transform target, Action onComplete)
    {
        isMoving = true;

        obj.SetParent(null);

        Vector3 startPos = obj.position;
        Quaternion startRot = obj.rotation;

        Vector3 endPos = target.position;
        Quaternion endRot = target.rotation;

        float distance = Vector3.Distance(startPos, endPos);

        float arcHeight = Mathf.Clamp(distance * arcMultiplier, minArcHeight, maxArcHeight);
        Vector3 midPoint = (startPos + endPos) / 2f + Vector3.up * arcHeight;

        float duration = Mathf.Lerp(minDuration, maxDuration, distance);
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = Mathf.Clamp01(time / duration);

            t = 1f - Mathf.Pow(1f - t, 3f);

            Vector3 pos =
                Mathf.Pow(1f - t, 2f) * startPos +
                2f * (1f - t) * t * midPoint +
                Mathf.Pow(t, 2f) * endPos;

            obj.position = pos;
            obj.rotation = Quaternion.Slerp(startRot, endRot, t);

            yield return null;
        }

        obj.position = endPos;
        obj.rotation = endRot;

        isMoving = false;

        onComplete?.Invoke();
    }
}