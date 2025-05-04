using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CursorManager : MonoBehaviour
{
    [Header("Cursor GameObjects (UI)")]
    public GameObject cursorIdle;
    public GameObject cursorGrab;
    public GameObject cursorTransition;
    public GameObject cursorOpen;

    [Header("Fade Settings")]
    public float fadeDuration = 0.1f;

    private Coroutine fadeRoutine;

    public void SetToOpen()
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(CursorSequence());
    }

    public void SetToIdle()
    {
        if (fadeRoutine != null) StopCoroutine(fadeRoutine);
        fadeRoutine = StartCoroutine(SwitchCursor(cursorIdle));
    }

    private IEnumerator CursorSequence()
    {
        yield return StartCoroutine(SwitchCursor(cursorGrab));
        yield return new WaitForSeconds(0.05f);
        yield return StartCoroutine(SwitchCursor(cursorTransition));
        yield return new WaitForSeconds(0.05f);
        yield return StartCoroutine(SwitchCursor(cursorOpen));
    }

    private IEnumerator SwitchCursor(GameObject target)
    {
        if (cursorIdle != target) cursorIdle.SetActive(false);
        if (cursorGrab != target) cursorGrab.SetActive(false);
        if (cursorTransition != target) cursorTransition.SetActive(false);
        if (cursorOpen != target) cursorOpen.SetActive(false);

        target.SetActive(true);

        Image img = target.GetComponent<Image>();
        Color transparent = new Color(1, 1, 1, 0);
        Color opaque = new Color(1, 1, 1, 1);
        img.color = transparent;

        float t = 0f;
        while (t < fadeDuration)
        {
            img.color = Color.Lerp(transparent, opaque, t / fadeDuration);
            t += Time.deltaTime;
            yield return null;
        }

        img.color = opaque;
    }

}