using System.Collections;
using UnityEngine;

public class Ring : MonoBehaviour
{
    [SerializeField] private float rotationDuration = 2f;

    private bool isRotating;
    public int ringIndex;

    private float currentYRotation;

    Outline outline;
    MeshCollider coll;

    private void Awake()
    {
        outline = GetComponent<Outline>();
        coll = GetComponent<MeshCollider>();
        outline.enabled = false;
    }

    private void OnMouseDown()
    {
        if (!RingPuzzleManager.Instance.canInteract) return;

        //Le aviso al manager que anillo es este
    }

    private void OnMouseEnter()
    {
        if (!RingPuzzleManager.Instance.canInteract) return;

        EnableOutline();
    }

    private void OnMouseExit()
    {
        if (!RingPuzzleManager.Instance.canInteract) return;

        DisableOutline();
    }

    public void DisableOutline()
    {
        outline.enabled = false;

        UIManager.Instance.ChangeCursor(false);
    }

    public void EnableOutline()
    {
        outline.enabled = true;
    }

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
