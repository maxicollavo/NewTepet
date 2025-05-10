using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Outline))]
[RequireComponent(typeof(BoxCollider))]
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
    public GameObject torchLight;
    private Light torchLightSource;
    private float originalLightIntensity;

    private void Awake()
    {
        anim = transform.parent.GetComponent<Animator>();
        outline = GetComponent<Outline>();
        coll = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        torchLightSource = torchLight.GetComponent<Light>();
        originalLightIntensity = torchLightSource.intensity;

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

        if (!ParticleSystem.isPlaying)
        {
            ParticleSystem.Play();
        }

        torchLight.SetActive(true);
        torchLightSource.enabled = true;

        Color startColor = main.startColor.color;
        float startIntensity = torchLightSource != null ? originalLightIntensity : 0f;
        float targetIntensity = (toAlpha == 0f) ? 0f : originalLightIntensity;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float currentAlpha = Mathf.Lerp(fromAlpha, toAlpha, t);

            Color newColor = new Color(startColor.r, startColor.g, startColor.b, currentAlpha);
            main.startColor = newColor;

            if (torchLightSource != null)
            {
                torchLightSource.intensity = Mathf.Lerp(startIntensity, targetIntensity, t);
            }

            yield return null;
        }

        Color finalColor = new Color(startColor.r, startColor.g, startColor.b, toAlpha);
        main.startColor = finalColor;

        torchLightSource.intensity = targetIntensity;

        if (Mathf.Approximately(toAlpha, 0f))
        {
            ParticleSystem.Stop();
            torchLight.SetActive(false);
        }
    }
}