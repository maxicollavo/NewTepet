using System.Collections;
using UnityEngine;

public class HandMecanism : MonoBehaviour, Interactor
{
    Outline outline;
    [SerializeField] Animator doorAnim;
    
    private void Awake()
    {
        outline = GetComponentInParent<Outline>();
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

    private void OpenDoor()
    {
        doorAnim.SetTrigger("Open");
        AudioManager.Instance.PlaySound("rocaMoviendose");
        UIManager.Instance.ChangeCursor(false);
    }

    public void Interact()
    {
        OpenDoor();
    }
}