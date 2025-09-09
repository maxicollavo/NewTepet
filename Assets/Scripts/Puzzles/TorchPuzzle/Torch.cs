using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Outline))]
[RequireComponent(typeof(BoxCollider))]
public class Torch : MonoBehaviour, Interactor
{
    public Action<Torch, int> OnInteractAction;
    public Action<Torch> OnAnimFinishAction;
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
    public GameObject ambientLight;
    private Light torchLightSource;
    private float originalLightIntensity;
    private Light ambientLightSource;
    private float originalAmbientIntensity;
    public AudioSource FireSound;

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

        if (ambientLight != null)
        {
            ambientLightSource = ambientLight.GetComponent<Light>();
            originalAmbientIntensity = ambientLightSource.intensity;
        }
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
            DisableParticleSystemAndHeatShader();
            anim.Play("Interact", 0, 0f);
            StartCoroutine(WaitForAnimationEnd(animDuration));

        }
        else
        {
            anim.Play("Interact_Reverse", 0, 0f);
            StartCoroutine(WaitForAnimationEnd(animDuration));
        }
    }

    public void OnAnimFinish()
    {
        OnAnimFinishAction?.Invoke(this);
    }

    void SendAction()
    {
        OnInteractAction?.Invoke(this, index);
    }

    private IEnumerator WaitForAnimationEnd(float duration)
    {
        yield return new WaitForSeconds(duration);
        coll.enabled = true;
        CanInteract = true;

        if (!IsUpsideDown)
        {
            EnableParticleSystemAndHeatShader();
        }
    }

    public void EnableParticleSystemAndHeatShader()
    {
        StartCoroutine(FadeParticleAlpha(0f, 1f, 1f)); // De alpha 0 a 1 en 1 segundo
    }

    public void DisableParticleSystemAndHeatShader()
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
            FireSound.Play();
        }

        //torchLight.SetActive(true);

        if (ambientLight != null)
            ambientLight.SetActive(true);

        torchLightSource.enabled = true;

        if (ambientLightSource != null)
            ambientLightSource.enabled = true;

        Color startColor = main.startColor.color;

        float startIntensity = originalLightIntensity;
        float startAmbient = originalAmbientIntensity;

        float targetIntensity = (toAlpha == 0f) ? 0f : originalLightIntensity;
        float targetAmbient = (toAlpha == 0f) ? 0f : originalAmbientIntensity;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float currentAlpha = Mathf.Lerp(fromAlpha, toAlpha, t);

            Color newColor = new Color(startColor.r, startColor.g, startColor.b, currentAlpha);
            main.startColor = newColor;

            if (torchLightSource != null)
                torchLightSource.intensity = Mathf.Lerp(startIntensity, targetIntensity, t);

            if (ambientLightSource != null)
                ambientLightSource.intensity = Mathf.Lerp(startAmbient, targetAmbient, t);

            yield return null;
        }

        Color finalColor = new Color(startColor.r, startColor.g, startColor.b, toAlpha);
        main.startColor = finalColor;

        torchLightSource.intensity = targetIntensity;

        if (ambientLightSource != null)
            ambientLightSource.intensity = targetAmbient;

        if (Mathf.Approximately(toAlpha, 0f))
        {
            ParticleSystem.Stop();
            //torchLight.SetActive(false);
            FireSound.Stop();

            if (ambientLight != null)
                ambientLight.SetActive(false);
        }
    }
}