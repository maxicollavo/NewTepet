using PDollarGestureRecognizer;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DrawingManager : MonoBehaviour
{
    //public DrawingState DrawingState;
    public float minDistance = 0.1f; // Distancia m�nima entre puntos
    [Range(0f, 1f)]
    public float recognitionThreshold = 0.8f; // Umbral m�nimo de puntuaci�n para un reconocimiento v�lido

    public TextMeshProUGUI resultText;

    public UnityEvent<Vector2> StartDrawingEvent;
    public UnityEvent<Result> EndRecognizeEvent;

    public GameObject lineRendererPrefab; // Prefab para crear nuevos LineRenderers
    public List<PatternSO> predefinedPatterns; // ScriptableObjects con patrones predefinidos

    private List<Gesture> gestureLibrary = new List<Gesture>();
    private List<Point> points = new List<Point>(); // Lista de puntos para todo el dibujo
    private List<LineRenderer> lineRenderers = new List<LineRenderer>(); // LineRenderers para cada stroke

    private int currentStrokeId = 0; // ID para identificar strokes separados
    private Camera mainCamera;

    void Start()
    {
        // Asigna la c�mara principal
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("No se encontr� una c�mara etiquetada como 'MainCamera'. Por favor, verifica tu configuraci�n.");
                return;
            }
        }

        // Cargar patrones predefinidos en la biblioteca de gestos
        if (predefinedPatterns == null || predefinedPatterns.Count == 0)
        {
            Debug.LogWarning("La lista predefinedPatterns est� vac�a o no tiene elementos asignados.");
            return;
        }

        foreach (var pattern in predefinedPatterns)
        {
            if (pattern != null)
            {
                gestureLibrary.Add(new Gesture(pattern.pattern.ToArray(), pattern.paternName,pattern.ID));
            }
            else
            {
                Debug.LogWarning("Un patr�n en predefinedPatterns es null.");
            }
        }
    }

    void Update()
    {


        if (StaticDrawingOptions.DrawingType == DrawingType.MultiStroke) MultipleStokeDrawing();
        else if (StaticDrawingOptions.DrawingType == DrawingType.SingleStroke) SingleStrokeDrawing();

    }

    private void SingleStrokeDrawing()
    {
        if (Input.GetMouseButtonDown(1))
        {
            StartDrawing();
        }
        else if (Input.GetMouseButton(1))
        {
            AddPoint();
        }
        else if (Input.GetMouseButtonUp(1)) // Clic derecho
        {
            RecognizeDrawing();
        }
    }

    private void MultipleStokeDrawing()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartDrawing();
        }
        else if (Input.GetMouseButton(0))
        {
            AddPoint();
        }
        else if (Input.GetMouseButtonUp(0))
        {
            EndStroke();
        }
        else if (Input.GetMouseButtonDown(1)) // Clic derecho
        {
            RecognizeDrawing();
        }
    }

    void StartDrawing()
    {
        // Crear un nuevo LineRenderer para el nuevo trazo
        GameObject lineRendererObject = Instantiate(lineRendererPrefab, transform);
        LineRenderer lineRenderer = lineRendererObject.GetComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lineRenderers.Add(lineRenderer);

        if (StaticDrawingOptions.DrawingType == DrawingType.SingleStroke) currentStrokeId = 1;

        // Evento de inicio de dibujo
        StartDrawingEvent.Invoke(mainCamera.ScreenToWorldPoint(Input.mousePosition));
    }

    void AddPoint()
    {
        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);

        if (points.Count == 0 || Vector2.Distance(mousePosition, new Vector2(points[^1].X, points[^1].Y)) > minDistance)
        {
            // A�adir punto con el strokeId actual
            points.Add(new Point(mousePosition.x, mousePosition.y, currentStrokeId));

            // Actualizar el LineRenderer actual
            LineRenderer currentLineRenderer = lineRenderers[^1];
            currentLineRenderer.positionCount++;
            currentLineRenderer.SetPosition(currentLineRenderer.positionCount - 1, mousePosition);
        }
    }

    void EndStroke()
    {
        // El trazo actual se finaliza al soltar el bot�n del mouse
        currentStrokeId++;
    }
    


    public void RecognizeDrawing()
    {
        if (points.Count == 0)
        {
            resultText.text = "No hay puntos para reconocer.";
            return;
        }

        // Crear un gesto a partir de los puntos dibujados
        Gesture userGesture = new Gesture(points.ToArray());

        // Reconocer el patr�n
        RecognizePattern(userGesture);


    }

    void RecognizePattern(Gesture userGesture)
    {
        // Cantidad de segmentos en el dibujo del usuario
        int userStrokeCount = currentStrokeId;

        // Filtrar patrones seg�n la cantidad de segmentos
        var filteredLibrary = new List<Gesture>();

        foreach (var pattern in predefinedPatterns)
        {
            if (pattern.StrokeAmmount == userStrokeCount)
            {
                var gesture = gestureLibrary.Find(g => g.Name == pattern.paternName);
                if (gesture != null)
                {
                    filteredLibrary.Add(gesture);
                }
            }
        }

        if (filteredLibrary.Count == 0)
        {
            // No hay patrones con la cantidad de segmentos requerida
            resultText.text = $"No hay patrones con {userStrokeCount} segmentos.";
            ClearDrawing();
            return;
        }

        // Clasificar el dibujo usando los patrones filtrados
        Result result = PointCloudRecognizer.Classify(userGesture, filteredLibrary.ToArray());

        if (!string.IsNullOrEmpty(result.GestureClass) && result.Score >= recognitionThreshold)
        {
            // Patr�n reconocido con puntuaci�n v�lida
            resultText.text = $"Patr�n reconocido: {result.GestureClass} ID: {result.GestureID} con puntuaci�n {result.Score:F2}";
            EndRecognizeEvent.Invoke(result);
        }
        else if (result.Score < recognitionThreshold)
        {
            // El patr�n no alcanz� el umbral de puntuaci�n
            resultText.text = $"Reconocimiento no v�lido: puntuaci�n {result.Score:F2} (m�nimo requerido: {recognitionThreshold})";
        }
        else
        {
            // No se reconoci� un patr�n v�lido
            resultText.text = "No se reconoci� ning�n patr�n.";
        }

        // Reiniciar el dibujo despu�s del reconocimiento
        ClearDrawing();
    }


    void ClearDrawing()
    {
        points.Clear();
        currentStrokeId = 0;

        // Eliminar todos los LineRenderers creados
        foreach (var lineRenderer in lineRenderers)
        {
            Destroy(lineRenderer.gameObject);
        }
        lineRenderers.Clear();
    }
}
