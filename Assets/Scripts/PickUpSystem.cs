using System;
using System.Collections;
using UnityEngine;

public class PickUpSystem : MonoBehaviour, Interactor
{
    private GameObject pickeable;
    private bool isInspecting = false;
    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Transform originalParent;
    private Vector3 originalScale;

    [Header("Inspección")]
    private bool hasReachedInspectionPoint = false;
    public Transform inspectionPoint;
    public float moveSpeed = 5f;
    public float rotationSpeed = 100f;
    public float inspectScale = 0.5f;

    private Outline outline;

    void Awake()
    {
        pickeable = transform.parent.gameObject;
        outline = transform.parent.GetComponent<Outline>();

        outline.enabled = false;
    }

    void Update()
    {
        if (isInspecting)
        {
            float dist = Vector3.Distance(pickeable.transform.position, inspectionPoint.position);
            float angle = Quaternion.Angle(pickeable.transform.rotation, inspectionPoint.rotation);

            if (!hasReachedInspectionPoint && (dist > 0.01f || angle > 1f))
            {
                pickeable.transform.position = Vector3.Lerp(pickeable.transform.position, inspectionPoint.position, Time.deltaTime * moveSpeed);
                pickeable.transform.rotation = Quaternion.Lerp(pickeable.transform.rotation, inspectionPoint.rotation, Time.deltaTime * moveSpeed);
                pickeable.transform.localScale = Vector3.Lerp(pickeable.transform.localScale, originalScale * 0.5f, Time.deltaTime * moveSpeed);
            }
            else
            {
                hasReachedInspectionPoint = true;

                if (Input.GetMouseButton(0))
                {
                    float rotX = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
                    float rotY = -Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;

                    pickeable.transform.Rotate(Camera.main.transform.up, rotY, Space.World);
                    pickeable.transform.Rotate(Camera.main.transform.right, rotX, Space.World);
                }

                pickeable.transform.localScale = Vector3.Lerp(pickeable.transform.localScale, originalScale * 0.5f, Time.deltaTime * moveSpeed);
            }

            if (Input.GetMouseButtonDown(1))
            {
                StopInspecting();
            }
        }
    }

    public void Interact()
    {
        if (isInspecting) return;
        EventManager.Instance.Dispatch(GameEventTypes.OnPickeable, this, EventArgs.Empty);
        hasReachedInspectionPoint = false;
        originalPosition = pickeable.transform.position;
        originalRotation = pickeable.transform.rotation;
        originalParent = pickeable.transform.parent;
        originalScale = pickeable.transform.localScale;

        if (pickeable.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = true;
        }

        pickeable.transform.parent = null;
        isInspecting = true;
    }

    public void StopInspecting()
    {
        isInspecting = false;

        StartCoroutine(ReturnToOriginal());
    }

    private IEnumerator ReturnToOriginal()
    {
        float t = 0;
        Vector3 startPos = pickeable.transform.position;
        Quaternion startRot = pickeable.transform.rotation;
        Vector3 startScale = pickeable.transform.localScale;

        while (t < 1)
        {
            t += Time.deltaTime * moveSpeed;
            pickeable.transform.position = Vector3.Lerp(startPos, originalPosition, t);
            pickeable.transform.rotation = Quaternion.Lerp(startRot, originalRotation, t);
            pickeable.transform.localScale = Vector3.Lerp(startScale, originalScale, t);
            yield return null;
        }

        pickeable.transform.parent = originalParent;

        if (TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = false;
        }

        EventManager.Instance.Dispatch(GameEventTypes.OnGameplay, this, EventArgs.Empty);
    }

    public void Aiming()
    {
        if (outline != null)
            outline.enabled = true;
    }

    public void DisableOutline()
    {
        if (outline != null)
            outline.enabled = false;
    }
}
