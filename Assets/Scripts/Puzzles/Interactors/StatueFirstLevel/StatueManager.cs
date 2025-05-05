using System;
using UnityEngine;

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
        statueInteractor.InteractorAction += OnStatueInteract;
        firstInteract = true;
    }

    private void OnStatueInteract(StatueInteractor interactor)
    {
        StatueInteract();
    }

    private void StatueInteract()
    {
        if (firstInteract)
        {
            onLeft = true;
            anim.SetBool("OnLeft", onLeft);
            anim.SetTrigger("Start");
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