using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Outline))]
public class MimicInteraction : MonoBehaviour, Interactor
{
    Outline outline;
    Animator anim;
    SphereCollider coll;

    public bool canInteract;

    [SerializeField] GameObject cam;

    private void Awake()
    {
        outline = GetComponent<Outline>();
        anim = GetComponent<Animator>();
        coll = GetComponent<SphereCollider>();
    }

    private void Start()
    {
        outline.enabled = false;
    }

    public void Interact()
    {
        if (!canInteract) return;

        StartCoroutine(OnInteraction());
    }

    IEnumerator OnInteraction()
    {
        DisableOutline();
        coll.enabled = false;
        TurnCamera(true);
        EventManager.Instance.Dispatch(GameEventTypes.OnCinematic, this, EventArgs.Empty);
        yield return new WaitForSeconds(GameManager.Instance.CameraTransitionTime);
        anim.SetTrigger("OnInteract");
        yield return new WaitForSeconds(7f);
        TurnCamera(false);
        yield return new WaitForSeconds(GameManager.Instance.CameraTransitionTime);
        coll.enabled = true;
    }

    public void DisableOutline()
    {
        outline.enabled = false;

        UIManager.Instance.ChangeCursor(false);
    }

    public void EnableOutline()
    {
        outline.enabled = true;
    }

    public void Aiming()
    {
        if (!canInteract) return;

        EnableOutline();

        UIManager.Instance.ChangeCursor(true);
    }

    private void TurnCamera(bool state)
    {
        if (state)
        {
            cam.SetActive(true);
        }
        else
        {
            cam.SetActive(false);
        }
    }

    public void SetSphereMaterials()
    {
        var renderer = GetComponent<MeshRenderer>();
        var materials = renderer.materials;

        materials[3].EnableKeyword("_EMISSION");

        Color originalEmission = materials[3].GetColor("_EmissionColor");
        materials[3].SetColor("_EmissionColor", originalEmission);

        Color glassColor = new Color(0f / 255f, 46f / 255f, 191f / 255f, 1f) * 4.816925f;
        materials[4].SetColor("_Color", glassColor);
        materials[4].SetFloat("_speed", 0.05f);

        renderer.materials = materials;
    }
}