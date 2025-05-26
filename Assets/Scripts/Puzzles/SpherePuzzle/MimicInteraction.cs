using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class MimicInteraction : MonoBehaviour, Interactor
{
    Outline outline;
    Animator anim;
    SphereCollider coll;

    [SerializeField] GameObject cam;

    private void Awake()
    {
        outline = GetComponent<Outline>();
        anim = GetComponent<Animator>();
        coll = GetComponent<SphereCollider>();
    }

    private void Start()
    {
        outline.enabled = false;
    }

    public void Interact()
    {
        StartCoroutine(OnInteraction());
    }

    IEnumerator OnInteraction()
    {
        DisableOutline();
        coll.enabled = false;
        TurnCamera(true);
        EventManager.Instance.Dispatch(GameEventTypes.OnCinematic, this, EventArgs.Empty);
        yield return new WaitForSeconds(1.3f);
        anim.SetTrigger("OnInteract");
        yield return new WaitForSeconds(7f);
        TurnCamera(false);
        yield return new WaitForSeconds(1.3f);
        coll.enabled = true;
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

    public void Aiming()
    {
        EnableOutline();

        UIManager.Instance.ChangeCursor(true);
    }

    private void TurnCamera(bool state)
    {
        if (state)
        {
            cam.SetActive(true);
        }
        else
        {
            cam.SetActive(false);
        }
    }
}