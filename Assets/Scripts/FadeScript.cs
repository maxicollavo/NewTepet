using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FadeScript : MonoBehaviour
{
    public GameObject canvasObject;
    public float fadeSpeed = 1f;
    public float displayTime = 1f;

    private Coroutine currentRoutine;
    private List<Graphic> graphics = new List<Graphic>();

    public void StartFadeTutorial()
    {
        ShowUI();
    }

    private void Awake()
    {
        graphics.AddRange(canvasObject.GetComponentsInChildren<Graphic>(true));
    }

    public void ShowUI()
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
        }

        currentRoutine = StartCoroutine(FadeSequence());
    }

    private IEnumerator FadeSequence()
    {
        canvasObject.SetActive(true);

        // Poner alfa en 0
        foreach (var g in graphics)
        {
            Color c = g.color;
            c.a = 0;
            g.color = c;
        }

        // Fade In
        float alpha = 0;
        while (alpha < 1)
        {
            alpha += Time.deltaTime * fadeSpeed;
            foreach (var g in graphics)
            {
                Color c = g.color;
                c.a = Mathf.Clamp01(alpha);
                g.color = c;
            }
            yield return null;
        }

        yield return new WaitForSeconds(displayTime);

        // Fade Out
        while (alpha > 0)
        {
            alpha -= Time.deltaTime * fadeSpeed;
            foreach (var g in graphics)
            {
                Color c = g.color;
                c.a = Mathf.Clamp01(alpha);
                g.color = c;
            }
            yield return null;
        }

        canvasObject.SetActive(false);
    }
}