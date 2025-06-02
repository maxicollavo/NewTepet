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
    [SerializeField] private float emissionIntensity;

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

    private void OnEnable()
    {
        SetSphereMaterials(SphereStates.Idle);
    }

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
        if (!canUse || isBeingHeld || hasWon) return;
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
        if (!canUse || isBeingHeld || hasWon) return;

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
        SetSphereMaterials(SphereStates.Win);
    }

    public enum SphereStates
    {
        Win,
        Idle,
        Loose
    }

    public void SetSphereMaterials(SphereStates state)
    {
        var renderer = GetComponent<MeshRenderer>();
        var materials = renderer.materials;

        Color emissionColor = Color.black;
        Color glassColor;

        switch (state)
        {
            case SphereStates.Win:
                materials[3].EnableKeyword("_EMISSION");
                emissionColor = new Color(1f, 235f / 255f, 0f) * emissionIntensity;
                glassColor = new Color(0f / 255f, 46f / 255f, 191f / 255f, 1f) * emissionIntensity;
                break;
            case SphereStates.Loose:
                materials[3].EnableKeyword("_EMISSION");
                emissionColor = Color.red * emissionIntensity;
                glassColor = Color.red * emissionIntensity;
                break;
            case SphereStates.Idle:
                // Desactivar emission
                materials[3].DisableKeyword("_EMISSION");
                glassColor = new Color(0f / 255f, 46f / 255f, 191f / 255f, 1f);
                break;
            default:
                Debug.LogWarning("Modo no reconocido. Usando el color azul por defecto.");
                materials[3].EnableKeyword("_EMISSION");
                emissionColor = materials[3].GetColor("_EmissionColor");
                glassColor = new Color(0f / 255f, 46f / 255f, 191f / 255f, 1f) * emissionIntensity;
                break;
        }

        if (state != SphereStates.Idle)
        {
            materials[3].SetColor("_EmissionColor", emissionColor);
        }

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
            StartCoroutine(ConfirmMovement(SphereStates.Win, SphereStates.Idle));
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
            //Cuando hacemos mal el movimiento
            StartCoroutine(ConfirmMovement(SphereStates.Loose, SphereStates.Idle));
        }
    }

    private IEnumerator ConfirmMovement(SphereStates first, SphereStates second)
    {
        canUse = false;
        SetSphereMaterials(first);
        yield return new WaitForSeconds(0.5f);

        if (!hasWon)
        {
            SetSphereMaterials(second);
        }

        canUse = true;
    }

}