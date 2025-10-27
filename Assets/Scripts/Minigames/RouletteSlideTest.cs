using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouletteSlideTest : MonoBehaviour, Interactor
{
    Outline outline;
    Animator touchButton;
    BoxCollider coll;

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
    }

    public void Interact()
    {
        StartCoroutine(TouchButtonCoroutine());
    }
}
