using System;
using System.Collections;
using UnityEngine;

public class Torch : MonoBehaviour, Interactor
{
    public Action<Torch, int> TorchAction;
    public bool IsUpsideDown => isUpsideDown;
    public AnimationEvent AnimationEvent;
    public int index;
    Animator anim;
    bool isUpsideDown;
    Outline outline;
    BoxCollider coll;
    private bool CanInteract = true;
    public ParticleSystem ParticleSystem;

    private void Awake()
    {
        anim = transform.parent.GetComponent<Animator>();
        outline = GetComponent<Outline>();
        coll = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        outline.enabled = false;
    }

    void EnableOutline()
    {
        outline.enabled = true;
    }

    public void Aiming()
    {
        if (!CanInteract) return;

        EnableOutline();

        UIManager.Instance.ChangeCursor(true);
    }

    public void DisableOutline()
    {
        outline.enabled = false;

        UIManager.Instance.ChangeCursor(false);
    }

    public void Interact()
    {
        if (!CanInteract) return;

        DisableOutline();
        CanInteract = false;
        isUpsideDown = !isUpsideDown;
        SendAction();

        float animDuration = anim.runtimeAnimatorController.animationClips[0].length;

        if (isUpsideDown)
        {
            DisableParticleSystem();
            anim.Play("Interact", 0, 0f);
            StartCoroutine(WaitForAnimationEnd(animDuration));
       
        }
        else
        {
            anim.Play("Interact_Reverse", 0, 0f);
            StartCoroutine(WaitForAnimationEnd(animDuration));
        }
    }

    void SendAction()
    {
        TorchAction?.Invoke(this, index);
    }

    private IEnumerator WaitForAnimationEnd(float duration)
    {
        yield return new WaitForSeconds(duration);
        coll.enabled = true;
        CanInteract = true;

        if (!IsUpsideDown)
        {
            EnableParticleSystem();
        }
    }

    public void EnableParticleSystem()
    {
        StartCoroutine(FadeParticleAlpha(0f, 1f, 1f)); // De alpha 0 a 1 en 1 segundo
    }

    public void DisableParticleSystem()
    {
        StartCoroutine(FadeParticleAlpha(1f, 0f, 0.3f)); // De alpha 1 a 0 en 1 segundo
    }

    private IEnumerator FadeParticleAlpha(float fromAlpha, float toAlpha, float duration)
    {
        var main = ParticleSystem.main;
        float elapsed = 0f;

        // Asegurarse que la particula esté activa
        if (!ParticleSystem.isPlaying)
            ParticleSystem.Play();

        Color startColor = main.startColor.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float currentAlpha = Mathf.Lerp(fromAlpha, toAlpha, t);

            Color newColor = new Color(startColor.r, startColor.g, startColor.b, currentAlpha);
            main.startColor = newColor;

            yield return null;
        }

        // Asegurar que el alpha final quede bien
        Color finalColor = new Color(startColor.r, startColor.g, startColor.b, toAlpha);
        main.startColor = finalColor;

        // Si el alpha final es 0, parar la particula
        if (Mathf.Approximately(toAlpha, 0f))
            ParticleSystem.Stop();
    }
}