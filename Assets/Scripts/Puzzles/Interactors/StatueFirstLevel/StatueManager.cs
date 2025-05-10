using System;
using UnityEngine;
using UnityEngine.Android;

public class StatueManager : MonoBehaviour
{
    [SerializeField] StatueInteractor statueInteractor;

    [SerializeField] Animator anim;
    [SerializeField] BoxCollider coll;

    private bool firstInteract;
    bool onLeft;

    public Action<StatueManager> StatueManagerAction;

    private void Start()
    {
        body.material = lightsOff;
        lights.SetActive(false);
        statueInteractor.InteractorAction += OnStatueInteract;
        firstInteract = true;
    }

    private void OnStatueInteract(StatueInteractor interactor)
    {
        StatueInteract();
    }

    [SerializeField] Renderer body;
    [SerializeField] GameObject lights;
    [SerializeField] Material lightsOn;
    [SerializeField] Material lightsOff;

    private void StatueInteract()
    {
        if (firstInteract)
        {
            onLeft = true;
            anim.SetBool("OnLeft", onLeft);
            anim.SetTrigger("Start");
            lights.SetActive(true);
            body.material = lightsOn;
            firstInteract = false;
            return;
        }

        onLeft = !onLeft;
        anim.SetBool("OnLeft", onLeft);
    }

    void SendActionToJeroglific()
    {
        StatueManagerAction?.Invoke(this);
    }

    public void SetCollider()
    {
        coll.enabled = true;
    }
}