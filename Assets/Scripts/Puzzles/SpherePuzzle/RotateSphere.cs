using System;
using System.Collections;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

public class RotateSphere : MonoBehaviour, Interactor
{
    [Header("Interacción")]
    private Outline outline;
    private bool canUse = true;
    public bool hasWon;
    private bool isBeingHeld;
    [SerializeField] ParticleSystem particle;
    [SerializeField] SphereCollider mimicSphereColl;

    [Header("Rotación")]
    private Transform pivot;
    [SerializeField] private float rotationSensitivity = 3f;

    [Header("Cinemachine")]
    [SerializeField] private GameObject puzzleCamera;

    [Header("Puzzle")]
    [SerializeField] private SpherePuzzleManager puzzleManager;
    [SerializeField] private Transform[] imagePoints;

    private int currentImageIndex = 0;
    private bool canScore = true;

    private void Awake()
    {
        outline = GetComponent<Outline>();
        pivot = GetComponent<Transform>();
    }

    private void Start()
    {
        outline.enabled = false;

        imagePoints = imagePoints.OrderBy(p => p.name).ToArray();
    }

    private void Update()
    {
        if (!isBeingHeld) return;

        if (Input.GetKey(KeyCode.A))
        {
            pivot.Rotate(Vector3.up, -rotationSensitivity, Space.World);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            pivot.Rotate(Vector3.up, rotationSensitivity, Space.World);
        }

        if (Input.GetKeyDown(KeyCode.Space))
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

    private void OnWinMethod()
    {
        Release();
        mimicSphereColl.enabled = false;
        canUse = false;
        hasWon = true;
        SetOnWinSphereMaterials("Win");
    }

    public void SetOnWinSphereMaterials(string state)
    {
        var renderer = GetComponent<MeshRenderer>();
        var materials = renderer.materials;

        Color emissionColor;
        Color glassColor;

        switch (state)
        {
            case "Win":
                emissionColor = materials[3].GetColor("_EmissionColor");
                glassColor = new Color(0f / 255f, 46f / 255f, 191f / 255f, 1f) * 4.816925f;
                break;
            case "Idle":
                emissionColor = Color.red * 4.816925f;
                glassColor = Color.red * 4.816925f;
                break;
            case "Loose":
                emissionColor = new Color(1f, 0.5f, 0f, 1f) * 4.816925f;
                glassColor = new Color(1f, 0.5f, 0f, 1f) * 4.816925f;
                break;
            default:
                Debug.LogWarning("Modo no reconocido. Usando el color azul por defecto.");
                emissionColor = materials[3].GetColor("_EmissionColor");
                glassColor = new Color(0f / 255f, 46f / 255f, 191f / 255f, 1f) * 4.816925f;
                break;
        }

        materials[3].EnableKeyword("_EMISSION");
        materials[3].SetColor("_EmissionColor", emissionColor);

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

        float proximityTolerance = 0.54f;

        if (distance < proximityTolerance)
        {
            currentImageIndex++;
            particle.Play();
            Debug.Log("Imagen detectada al frente, avanzando...");

            if (currentImageIndex >= imagePoints.Length)
            {
                canScore = false;
                puzzleManager.OnWin();
                OnWinMethod();
            }
        }
        else
        {
            currentImageIndex = 0;
            StartCoroutine(WrongMovement());
        }
    }

    private IEnumerator WrongMovement()
    {
        //Cambiar el emmisive del mat a rojo
        SetOnWinSphereMaterials("Win");
        //Deshabilitamos la esfera
        canUse = false;
        yield return new WaitForSeconds(0.5f);
        //Cambiar el emmisive del mat al original
        //outline.OutlineColor = originalColor;
        DisableOutline();
    }
}