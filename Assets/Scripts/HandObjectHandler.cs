using UnityEngine;

public class HandObjectHandler : MonoBehaviour
{
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private void Awake()
    {
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
    }

    public void Reset()
    {
        transform.localPosition = originalPosition;
        transform.localRotation = originalRotation;
    }
}
