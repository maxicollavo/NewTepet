using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class StatueManager : MonoBehaviour
{
    [SerializeField] StatueInteractor statueInteractor;
    [SerializeField] OwlEvents owlEvents;
    public Action<StatueManager, int> SetNodes;

    [SerializeField] Animator anim;
    [SerializeField] BoxCollider coll;

    private bool firstInteract;
    public bool onLeft;
    int owlPos;
    private Coroutine intensityRoutine;
    private float intensityLerpDuration = 0.15f;

    public Action<StatueManager> StatueManagerAction;

    private void Start()
    {
        body.material = lightsOff;
        statueInteractor.InteractorAction += OnStatueInteract;
        owlEvents.AnimFinishAction += OnAnimFinish;
        firstInteract = true;

        if (lights != null)
            lights.SetActive(false);
    }

    private void OnAnimFinish(OwlEvents events, int pos)
    {
        float targetIntensity = pos == 0 ? firstInt : secondInt;
        owlPos = pos;

        if (intensityRoutine != null)
            StopCoroutine(intensityRoutine);

        intensityRoutine = StartCoroutine(LerpLightIntensity(blueLight, targetIntensity, intensityLerpDuration));
    }

    private IEnumerator LerpLightIntensity(Light light, float targetIntensity, float duration)
    {
        float startIntensity = light.intensity;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            light.intensity = Mathf.Lerp(startIntensity, targetIntensity, time / duration);
            yield return null;
        }

        light.intensity = targetIntensity;

        SetNodes?.Invoke(this, owlPos);
    }


    private void OnStatueInteract(StatueInteractor interactor)
    {
        StatueInteract();
    }

    [SerializeField] Renderer body;
    [SerializeField] GameObject lights;
    [SerializeField] Light blueLight;
    [SerializeField] float firstInt;
    [SerializeField] float secondInt;
    [SerializeField] Material lightsOn;
    [SerializeField] Material lightsOff;

    private void StatueInteract()
    {
        if (firstInteract)
        {
            onLeft = !onLeft;
            anim.SetBool("OnLeft", onLeft);
            anim.SetTrigger("Start");
            body.material = lightsOn;
            firstInteract = false;

            if (lights != null)
                lights.SetActive(true);
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