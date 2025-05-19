using System.Collections.Generic;
using UnityEngine;

public class ActivateLasers : MonoBehaviour, Interactor
{
    [SerializeField] List<GameObject> lasers;
    [SerializeField] GameObject roomLight;

    Outline outline;
    BoxCollider boxCol;
    Animator anim;

    private void Awake()
    {
        outline = GetComponent<Outline>();
        boxCol = GetComponent<BoxCollider>();
        anim = GetComponent<Animator>();

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
        anim.SetTrigger("Interact");
        roomLight.SetActive(false);
        boxCol.enabled = false;
        outline.enabled = false;
        foreach (var laser in lasers)
            laser.SetActive(true);
        UIManager.Instance.ChangeCursor(false);
    }
}
