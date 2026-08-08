using System;
using System.Collections;
using UnityEngine;

public class HandMecanism : MonoBehaviour, Interactor
{
    Outline outline;
    [SerializeField] Animator doorAnim;
    Animator touchButton;
    BoxCollider coll;
    [SerializeField] AudioSource door3DSound;

    private void Awake()
    {
        outline = GetComponent<Outline>();
        touchButton = GetComponent<Animator>();
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

    private IEnumerator TouchButtonCoroutine()
    {
        coll.enabled = false;
        UIManager.Instance.ChangeCursor(false);
        touchButton.SetTrigger("Interact");
        DisableOutline();
        yield return new WaitForSeconds(1f);
        doorAnim.SetTrigger("Open");
        EventManager.Instance.Dispatch(GameEventTypes.OnDoorOpen, this, EventArgs.Empty);
        door3DSound.Play();
        //AudioManager.Instance.PlaySound("rocaMoviendose");
    }

    public void Interact()
    {
        StartCoroutine(TouchButtonCoroutine());
    }
}