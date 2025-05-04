using System.Collections.Generic;
using UnityEngine;

public class PatternTracker : MonoBehaviour
{
    [Header("Configuraci�n")]
    public List<GameObject> validNodes;
    public LayerMask detectionLayer;

    private List<GameObject> currentPath = new List<GameObject>();
    public bool isTracking { get; private set; }
    public Camera trackCamera;
    [SerializeField] TrailManager manager;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            currentPath.Clear();
            isTracking = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isTracking = false;
            CheckPattern();
        }

        if (isTracking)
        {
            TrackMouse();
        }
    }

    void TrackMouse()
    {
        Ray ray = trackCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            GameObject hitObj = hit.collider.gameObject;

            if (validNodes.Contains(hitObj))
            {
                if (!currentPath.Contains(hitObj))
                {
                    currentPath.Add(hitObj);
                    Debug.Log($"Nodo v�lido agregado: {hitObj.name}");

                    if (currentPath.Count == validNodes.Count)
                    {
                        isTracking = false;
                        CheckPattern();
                    }
                }
            }
            else
            {
                Debug.Log($"Nodo inv�lido tocado: {hitObj.name}. Reiniciando patr�n.");
                currentPath.Clear();
                isTracking = false;
            }
        }
    }

    void CheckPattern()
    {
        if (currentPath.Count == validNodes.Count)
        {
            isTracking = false;
        }
        else
        {
            Debug.Log("Patr�n incompleto.");
        }
    }
}
