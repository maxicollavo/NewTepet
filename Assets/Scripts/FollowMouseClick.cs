using System;
using System.Collections;
using UnityEngine;

public class FollowMouseClick : MonoBehaviour
{
    public Transform emitterObject;
    LineRenderer lineRenderer;

    private Camera cam;
    float fixedZ;

    private bool onPause;

    private void OnEnable()
    {
        NewEventManager.OnPaused += PauseTriggered;
    }

    private void OnDisable()
    {
        lineRenderer.enabled = false;
        NewEventManager.OnPaused -= PauseTriggered;
    }

    private void PauseTriggered(bool state)
    {
        onPause = state;
        Debug.Log(state);
    }

    private void Awake()
    {
        lineRenderer = emitterObject.gameObject.GetComponent<LineRenderer>();
    }

    private IEnumerator Start()
    {
        cam = Camera.main;
        fixedZ = transform.position.z;
        lineRenderer.positionCount = 2;

        yield return null; // espera un frame
        gameObject.SetActive(false);
    }


    void Update()
    {
        if (!gameObject.activeInHierarchy || onPause) return;

        if (Input.GetMouseButton(0))
        {
            lineRenderer.enabled = true;
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Mathf.Abs(cam.transform.position.z - fixedZ);

            Vector3 worldPos = cam.ScreenToWorldPoint(mousePos);
            worldPos.z = fixedZ;

            transform.position = worldPos;

            if (lineRenderer != null && emitterObject != null)
            {
                lineRenderer.SetPosition(0, emitterObject.position);
                lineRenderer.SetPosition(1, worldPos);
            }
        }
        else if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, Vector3.zero);
            lineRenderer.SetPosition(1, Vector3.zero);
        }
    }
}