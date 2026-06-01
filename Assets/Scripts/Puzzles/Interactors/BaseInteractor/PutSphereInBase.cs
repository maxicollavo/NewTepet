using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class PutSphereInBase : MonoBehaviour, Interactor
{
    Outline outline;
    BoxCollider coll;

    [SerializeField] ObjectsToPick requiredObj;

    [SerializeField] private Transform sphereInHand;
    [SerializeField] private Transform spherePivot;

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
        ParabolaController.Instance.FollowParabolaTo(sphereInHand, spherePivot);
        PickedObjData.Instance.MarkAsThrowed(requiredObj, false);
        coll.enabled = false;
        DisableOutline();
        yield return new WaitForSeconds(0.5f);
        sphere.gameObject.SetActive(true);
        sphereInHand.gameObject.SetActive(false);
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
