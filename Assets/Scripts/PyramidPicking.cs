using System;
using UnityEngine;

public class PyramidPicking : MonoBehaviour, Interactor
{
    public Action<PyramidPicking> OnPicking;

    [SerializeField] Outline outline;

    [SerializeField] GameObject handPyramid;
    [SerializeField] GameObject grabbedPyramid;
    [SerializeField] GameObject coneLight;
    [SerializeField] Animator door;

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

    private void GrabPyramid()
    {
        OnPicking?.Invoke(this);
        handPyramid.SetActive(true);
        grabbedPyramid.SetActive(false);
        coneLight.SetActive(false);
        AudioManager.Instance.PlaySound("Grab");
        UIManager.Instance.ChangeCursor(false);
    }

    public void Interact()
    {
        GrabPyramid();
    }
}