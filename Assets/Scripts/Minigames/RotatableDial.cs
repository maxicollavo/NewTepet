using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class RotatableDial : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    private RectTransform rectTransform;
    private Camera eventCamera;
    private float startAngle;

    public bool canRotateDial;

    private static RotatableDial currentlySelectedDial;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!canRotateDial)
        {
            return;
        }

        eventCamera = eventData.pressEventCamera;
        startAngle = GetMouseAngle(eventData);
        currentlySelectedDial = this;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!canRotateDial)
            return;

        if (currentlySelectedDial == this)
        {
            currentlySelectedDial = null;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!canRotateDial)
        {
            return;
        }

        float currentAngle = GetMouseAngle(eventData);
        float deltaAngle = currentAngle - startAngle;

        rectTransform.Rotate(0, 0, deltaAngle);
        startAngle = currentAngle;
    }

    private float GetMouseAngle(PointerEventData eventData)
    {
        if (eventData == null)
        {
            return 0f;
        }

        if (!RectTransformUtility.ScreenPointToWorldPointInRectangle(
            rectTransform,
            eventData.position,
            eventCamera,
            out Vector3 worldPoint))
        {
            Debug.LogWarning("[RotatableDial] No se pudo convertir ScreenPoint -> WorldPoint.");
        }

        Vector2 dir = worldPoint - rectTransform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        return angle;
    }
}
