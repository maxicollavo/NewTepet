using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeBlackImage : MonoBehaviour
{
    private Image _sceneFadeImage;

    public static FadeBlackImage Instance;

    private void Awake()
    {
        _sceneFadeImage = GetComponent<Image>();

        Instance = this;
    }

    public void StartFadeIn(float duration)
    {
        gameObject.SetActive(true);
        _sceneFadeImage.color = new Color(_sceneFadeImage.color.r, _sceneFadeImage.color.g, _sceneFadeImage.color.b, 1f);
        StartCoroutine(FadeOutCoroutine(duration));
    }

    private IEnumerator FadeOutCoroutine(float duration)
    {
        Color startColor = _sceneFadeImage.color;
        Color targetColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        yield return FadeCoroutine(startColor, targetColor, duration);
    }

    private IEnumerator FadeCoroutine(Color startColor, Color targetColor, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            _sceneFadeImage.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
            elapsedTime += Time.deltaTime;
        }

        _sceneFadeImage.color = targetColor;
    }
}
