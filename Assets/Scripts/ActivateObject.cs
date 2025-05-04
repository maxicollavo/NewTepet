using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateObject : MonoBehaviour, Interactor
{
    [SerializeField] Animator anim;

    public void Aiming()
    {
        throw new System.NotImplementedException();
    }

    public void DisableOutline()
    {
        throw new System.NotImplementedException();
    }

    public void Interact()
    {
        Debug.Log("Interactua");
        anim.SetTrigger("Interact");
    }
}