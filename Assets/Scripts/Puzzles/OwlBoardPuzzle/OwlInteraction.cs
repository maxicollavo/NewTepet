using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OwlInteraction : MonoBehaviour, Interactor
{
    Outline outline;
    [SerializeField] Animator anim;
    [SerializeField] GameObject lights;
    [SerializeField] GameObject[] rightFigures;
    [SerializeField] GameObject[] leftFigures;

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
    }

    public void OwlOnRight()
    {
        foreach (var figure in rightFigures)
        {
            figure.SetActive(true);
        }
    }

    public void TurnOffRight()
    {
        foreach (var figure in rightFigures)
        {
            figure.SetActive(false);
        }
    }

    public void OwlOnLeft()
    {
        foreach (var figure in leftFigures)
        {
            figure.SetActive(true);
        }
    }

    public void TurnOffLeft()
    {
        foreach (var figure in leftFigures)
        {
            figure.SetActive(false);
        }
    }
}
