using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RotateSphere : MonoBehaviour, Interactor
{
    private Outline outline;
    private bool canUse = true;

    [SerializeField] Transform forwardTarget;
    [SerializeField] Transform lookAtTarget;
    [SerializeField] float rotationSpeed = 5f;

    private int currentTargetIndex = 0;
    private bool isRotating = false;

    private void Awake()
    {
        outline = GetComponent<Outline>();
    }

    private void Start()
    {
        outline.enabled = false;
    }

    public void DisableOutline()
    {
        outline.enabled = false;
        UIManager.Instance.ChangeCursor(false);
    }

    void EnableOutline()
    {
        outline.enabled = true;
    }

    public void Aiming()
    {
        if (!canUse) return;

        EnableOutline();
        UIManager.Instance.ChangeCursor(true);
    }

    public void Interact()
    {
        //if (!canUse || isRotating || forwardTarget.Count == 0) return;

        DisableOutline();
        //StartCoroutine(RotateToNextTarget());
    }

    //private IEnumerator RotateToNextTarget()
    //{
    //    isRotating = true;

    //    Transform target = forwardTarget[currentTargetIndex];

    //    Vector3 directionToTarget = (target.position - lookAtTarget.position);
    //    directionToTarget.y = 0f;

    //    Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
    //    Quaternion initialRotation = transform.rotation;

    //    float elapsed = 0f;
    //    while (elapsed < 1f)
    //    {
    //        transform.rotation = Quaternion.Slerp(initialRotation, targetRotation, elapsed);
    //        elapsed += Time.deltaTime * rotationSpeed;
    //        yield return null;
    //    }

    //    transform.rotation = targetRotation;

    //    currentTargetIndex = (currentTargetIndex + 1) % forwardTarget.Count;
    //    isRotating = false;
    //}
}
