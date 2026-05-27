using System;
using System.Collections;
using UnityEngine;

public class PutSphereInBase : MonoBehaviour, Interactor
{
    Outline outline;
    BoxCollider coll;

    [SerializeField] ObjectsToPick requiredObj;
    [SerializeField] PlaceObject placeObject;
    [SerializeField] HandObjectHandler handObjectHandler;

    [SerializeField] GameObject CM_PuzzleCamera;

    [SerializeField] Transform baseSlot;
    private bool sphereInBase;

    private void Awake()
    {
        outline = GetComponent<Outline>();
        coll = GetComponent<BoxCollider>();
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
        EnableOutline();
        UIManager.Instance.ChangeCursor(true);
    }

    public void Interact()
    {
        if (PickedObjData.Instance.WasPicked(requiredObj))
        {
            StartCoroutine(EnterPuzzleCoroutine());
        }
        else
        {
            StartCoroutine(CannotPick());
        }
    }

    [SerializeField] RotateSphere sphere;

    public IEnumerator EnterPuzzleCoroutine()
    {
        if (!sphereInBase)
        {
            PickedObjData.Instance.MarkAsThrowed(requiredObj, false);

            yield return StartCoroutine(MoveSphereToSlot(sphere.transform, baseSlot));

            DisableOutline();

            yield return new WaitForSeconds(0.5f);

            PlaceObject();

            EnableOutline();
        }
        else
        {
            coll.enabled = false;

            TurnPuzzleCamera(true);

            yield return new WaitForSeconds(1.5f);

            sphere.onBase = true;

            EventManager.Instance.Dispatch(GameEventTypes.OnPuzzle, this, EventArgs.Empty);
        }
    }

    IEnumerator MoveSphereToSlot(Transform obj, Transform target)
    {
        obj.SetParent(null);

        Vector3 startPos = obj.position;
        Quaternion startRot = obj.rotation;

        Vector3 endPos = target.position;
        Quaternion endRot = target.rotation;

        float distance = Vector3.Distance(startPos, endPos);

        float arcHeight = Mathf.Clamp(distance * 0.3f, 0.15f, 0.5f);
        Vector3 midPoint = (startPos + endPos) / 2 + Vector3.up * arcHeight;

        float duration = Mathf.Lerp(0.2f, 0.4f, distance);
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            t = 1 - Mathf.Pow(1 - t, 3);

            Vector3 pos =
                Mathf.Pow(1 - t, 2) * startPos +
                2 * (1 - t) * t * midPoint +
                Mathf.Pow(t, 2) * endPos;

            obj.position = pos;

            obj.rotation = Quaternion.Slerp(startRot, endRot, t);

            yield return null;
        }

        obj.position = endPos;
        obj.rotation = endRot;
    }

    public void PlaceObject()
    {
        placeObject.Place();
        handObjectHandler.Reset();
    }

    private void TurnPuzzleCamera(bool state)
    {
        if (state)
        {
            CM_PuzzleCamera.SetActive(true);
        }
        else
        {
            CM_PuzzleCamera.SetActive(false);
        }
    }

    private IEnumerator CannotPick()
    {
        EnableOutline();

        var originalColor = outline.OutlineColor;

        outline.OutlineColor = Color.red;

        yield return new WaitForSeconds(0.5f);

        outline.OutlineColor = originalColor;

        DisableOutline();
    }
}