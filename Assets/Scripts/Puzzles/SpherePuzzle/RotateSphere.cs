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
    private bool isBeingHeld = true;
    [SerializeField] ParticleSystem particle;
    [SerializeField] SphereCollider mimicSphereColl;
    [SerializeField] private float emissionIntensity;
    Material fillMat;
    private float fillAmount;
    private int fillCounter;
    public GameObject uiPuzzle;

    [Header("Rotación")]
    private Transform pivot;
    [SerializeField] private float rotationSensitivity = 3f;

    [Header("Cinemachine")]
    [SerializeField] private GameObject puzzleCamera;

    [Header("Puzzle")]
    [SerializeField] private SpherePuzzleManager puzzleManager;
    [SerializeField] private Transform[] winImagePoints;
    [SerializeField] private Transform[] allImages;

    private int currentImageIndex = 0;
    private bool canScore = true;

    private void OnEnable()
    {
        SetSphereMaterials(SphereStates.Idle);

        var renderer = GetComponent<MeshRenderer>();
        fillMat = renderer.materials[5];
    }

    private void Awake()
    {
        outline = GetComponent<Outline>();
        pivot = GetComponent<Transform>();
    }

    private void Start()
    {
        outline.enabled = false;

        winImagePoints = winImagePoints.OrderBy(p => p.name).ToArray();
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
        uiPuzzle.SetActive(true);
        EventManager.Instance.Dispatch(GameEventTypes.OnPuzzle, this, EventArgs.Empty);

        isBeingHeld = true;
    }

    private void Release()
    {
        isBeingHeld = false;
        puzzleCamera.SetActive(false);
        uiPuzzle.SetActive(false);
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

    private Transform GetClosestImage()
    {
        float closestDistance = Mathf.Infinity;
        Transform closestImage = null;

        foreach (var image in allImages)
        {
            float distance = Vector3.Distance(image.position, puzzleCamera.transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestImage = image;
            }
        }

        return closestImage;
    }

    private void RotateAndCenterCurrentImage()
    {
        var currentImage = GetClosestImage();

        Vector3 direccionDeseada = puzzleCamera.transform.position - this.transform.position;
        direccionDeseada.y = 0;
        direccionDeseada.Normalize();

        Vector3 direccionActual = currentImage.position - this.transform.position;
        direccionActual.y = 0;
        direccionActual.Normalize();

        Quaternion rotacionNecesaria = Quaternion.FromToRotation(direccionActual, direccionDeseada);

        this.transform.rotation = rotacionNecesaria * this.transform.rotation;
    }

    private void CheckFillAmount()
    {
        switch (fillCounter)
        {
            case 0:
                fillMat.SetFloat("_FillAmount", 0f);
                break;
            case 1:
                fillMat.SetFloat("_FillAmount", 0.28f);
                break;
            case 2:
                fillMat.SetFloat("_FillAmount", 0.35f);
                break;
            case 3:
                fillMat.SetFloat("_FillAmount", 1f);
                break;
            default:
                break;
        }
    }


    private void CheckFrontImage()
    {
        if (!canScore || currentImageIndex >= winImagePoints.Length) return;

        RotateAndCenterCurrentImage();

        Transform currentImage = winImagePoints[currentImageIndex];
        float distance = Vector3.Distance(puzzleCamera.transform.position, currentImage.position);
        float proximityTolerance = 0.54f;

        if (distance < proximityTolerance)
        {
            currentImageIndex++;
            fillCounter++;
            particle.Play();
            StartCoroutine(ConfirmMovement(SphereStates.Win, SphereStates.Idle));

            if (currentImageIndex >= winImagePoints.Length)
            {
                canScore = false;
                puzzleManager.OnWin();
                OnWinMethod();
            }
        }
        else
        {
            currentImageIndex = 0;
            fillCounter = 0;
            //Cuando hacemos mal el movimiento
            StartCoroutine(ConfirmMovement(SphereStates.Loose, SphereStates.Idle));
        }

        CheckFillAmount();
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