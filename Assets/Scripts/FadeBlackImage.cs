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

    public void StartFadeToBlack(float duration)
    {
        _sceneFadeImage.color = new Color(0f, 0f, 0f, 0f);
        StartCoroutine(FadeInCoroutine(duration));
    }

    private IEnumerator FadeInCoroutine(float duration)
    {
        Color startColor = new Color(0f, 0f, 0f, 0f);
        Color targetColor = new Color(0f, 0f, 0f, 1f);

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
