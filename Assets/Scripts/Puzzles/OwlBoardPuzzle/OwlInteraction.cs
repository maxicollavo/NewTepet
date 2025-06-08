using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OwlInteraction : MonoBehaviour, Interactor
{
    Outline outline;
    [SerializeField] Animator anim;
    [SerializeField] GameObject lights;
    [SerializeField] GameObject[] rightDecals;
    [SerializeField] GameObject[] leftDecals;

    bool onRight;
    bool isFirstInteraction = true;

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
        EnableOutline();
        UIManager.Instance.ChangeCursor(true);
    }

    public void Interact()
    {
        OnInteraction();
    }

    private void OnInteraction()
    {
        if (isFirstInteraction)
        {
            anim.SetTrigger("FirstInteraction");
            onRight = true;
            isFirstInteraction = false;
            lights.SetActive(true);
            return;
        }

        onRight = !onRight;
        anim.SetBool("Rotate", onRight);

        if (onRight)
        {
            foreach (var decal in leftDecals)
            {
                if (decal.activeInHierarchy)
                    decal.SetActive(false);
                else
                    decal.SetActive(true);
            }
        }
        else
        {
            foreach (var decal in rightDecals)
            {
                if (decal.activeInHierarchy)
                    decal.SetActive(false);
                else
                    decal.SetActive(true);
            }
        }
    }

    public void OwlOnRight()
    {
        foreach (var decal in rightDecals)
        {
            if (decal.activeInHierarchy)
                decal.SetActive(false);
            else
                decal.SetActive(true);
        }
    }

    public void OwlOnLeft()
    {
        foreach (var decal in leftDecals)
        {
            if (decal.activeInHierarchy)
                decal.SetActive(false);
            else
                decal.SetActive(true);
        }
    }
}
