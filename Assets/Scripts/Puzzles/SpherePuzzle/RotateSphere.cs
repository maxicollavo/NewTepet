using System;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

public class RotateSphere : MonoBehaviour, Interactor
{
    [Header("Interacción")]
    private Outline outline;
    private SphereCollider coll;
    private bool canUse = true;
    public bool hasWon;
    private bool isBeingHeld;

    [Header("Rotación")]
    private Transform pivot;
    [SerializeField] private float rotationSensitivity = 3f;

    [Header("Cinemachine")]
    [SerializeField] private GameObject puzzleCamera;
    private Vector3 cameraMidPoint;

    [Header("Puzzle")]
    [SerializeField] private SpherePuzzleManager puzzleManager;
    [SerializeField] private WallLaser laser;
    [SerializeField] private Transform[] imagePoints; // Puntos de referencia para las imágenes
    [SerializeField] private float frontTolerance = 15f; // Ángulo de tolerancia

    private Vector2 lastMousePos;
    private int currentImageIndex = 0;
    private bool canScore = true;

    private void Awake()
    {
        outline = GetComponent<Outline>();
        pivot = GetComponent<Transform>();
        coll = GetComponent<SphereCollider>();
    }

    private void Start()
    {
        outline.enabled = false;
        puzzleManager.OnWinAction += OnWinMethod;

        // Ordena las imágenes por nombre para asegurarse del orden deseado
        imagePoints = imagePoints.OrderBy(p => p.name).ToArray();
    }

    private void Update()
    {
        if (!isBeingHeld) return;

        if (Input.GetMouseButton(0))
        {
            float mouseDelta = Input.GetAxis("Mouse X");
            pivot.Rotate(Vector3.up, -mouseDelta * rotationSensitivity, Space.World);
        }

        if (Input.GetMouseButtonUp(0))
        {
            CheckFrontImage();
        }

        if (Input.GetMouseButtonDown(1))
        {
            Release();
        }
    }

    public void Aiming()
    {
        if (!canUse || isBeingHeld) return;
        EnableOutline();
    }

    public void EnableOutline()
    {
        outline.enabled = true;
        UIManager.Instance.ChangeCursor(true);
    }

    public void DisableOutline()
    {
        outline.enabled = false;
        UIManager.Instance.ChangeCursor(false);
    }

    public void Interact()
    {
        if (!canUse || isBeingHeld) return;

        DisableOutline();
        puzzleCamera.SetActive(true);
        EventManager.Instance.Dispatch(GameEventTypes.OnPuzzle, this, EventArgs.Empty);

        isBeingHeld = true;
    }

    private void Release()
    {
        isBeingHeld = false;
        puzzleCamera.SetActive(false);
        EventManager.Instance.Dispatch(GameEventTypes.OnGameplay, this, EventArgs.Empty);
    }

    private void OnWinMethod(SpherePuzzleManager manager)
    {
        Release();
        canUse = false;
        hasWon = true;

        if (!laser.isEnabled) return;
        SetOnWinSphereMaterials();
    }

    public void SetOnWinSphereMaterials()
    {
        var renderer = GetComponent<MeshRenderer>();
        var materials = renderer.materials;

        materials[3].EnableKeyword("_EMISSION");

        Color originalEmission = materials[3].GetColor("_EmissionColor");
        materials[3].SetColor("_EmissionColor", originalEmission);

        Color glassColor = new Color(0f / 255f, 46f / 255f, 191f / 255f, 1f) * 4.816925f;
        materials[4].SetColor("_Color", glassColor);
        materials[4].SetFloat("_speed", 0.05f);

        renderer.materials = materials;
    }

    private void CheckFrontImage()
    {
        if (!canScore || currentImageIndex >= imagePoints.Length) return;

        Transform currentImage = imagePoints[currentImageIndex];

        float distance = Vector3.Distance(puzzleCamera.transform.position, currentImage.position);

        Debug.Log("Distancia al frente de la cámara: " + distance);

        float proximityTolerance = 0.54f; // Puedes ajustar este valor

        if (distance < proximityTolerance)
        {
            currentImageIndex++;
            Debug.Log("Imagen detectada al frente, avanzando...");

            if (currentImageIndex >= imagePoints.Length)
            {
                canScore = false;
                puzzleManager.OnWin();
            }
        }
    }
}