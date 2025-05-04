using UnityEngine;
using TMPro;

public class ResizePanelToText : MonoBehaviour
{
    public TextMeshProUGUI textMeshPro;
    public RectTransform panel;

    public float maxWidth = 400f;
    public float minWidth = 100f;
    public float paddingX = 20f;
    public float paddingY = 10f;

    void Update()
    {
        Vector2 textSize = textMeshPro.GetPreferredValues();

        float finalWidth = Mathf.Clamp(textSize.x + paddingX, minWidth, maxWidth);
        float finalHeight = textSize.y + paddingY;

        panel.sizeDelta = new Vector2(finalWidth, finalHeight);

        panel.position = textMeshPro.rectTransform.position;
    }
}