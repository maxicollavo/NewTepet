using System;
using System.Collections;
using System.Linq;
using UnityEngine;

public class RotateSphere : MonoBehaviour, Interactor
{
    [Header("Interacción")]
    private Outline outline;
    private bool canUse = true;
    [HideInInspector] public bool hasWon;
    [HideInInspector] public bool isBeingHeld;
    [SerializeField] ParticleSystem particle;
    [SerializeField] private float emissionIntensity;
    Material fillMat;
    private float fillAmount;
    private int fillCounter;
    public GameObject uiPuzzle;
    public float proximityTolerance;
    Animator animator;

    [Header("Rotación")]
    private Transform pivot;
    Animator anim;

    [Header("Cinemachine")]
    [SerializeField] private GameObject puzzleCamera;

    [Header("Puzzle")]
    [SerializeField] private SpherePuzzleManager puzzleManager;
    [SerializeField] private Transform[] winImagePoints;
    [SerializeField] private Transform[] allImages;

    [Header("Fills")]
    [SerializeField] float firstFillAmount;
    [SerializeField] float secondFillAmount;
    [SerializeField] float thirdFillAmount;

    private int currentImageIndex = 0;
    private bool canScore = true;

    Transform child;

    private void OnEnable()
    {
        child = transform.GetChild(0);

        var renderer = child.GetComponent<MeshRenderer>();
        var materials = renderer.materials;

        if (materials.Length > 2)
        {
            Material[] trimmedMaterials = new Material[2];
            trimmedMaterials[0] = materials[0];
            trimmedMaterials[1] = materials[1];
            renderer.materials = trimmedMaterials;
        }

        fillMat = renderer.materials[1];

        //SetSphereMaterials(SphereStates.Idle);
    }

    private void Awake()
    {
        outline = GetComponent<Outline>();
        pivot = GetComponent<Transform>();
        anim = GetComponent<Animator>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        outline.enabled = false;
        animator.enabled = false;

        winImagePoints = winImagePoints.OrderBy(p => p.name).ToArray();
    }

    private void Update()
    {
        if (!isBeingHeld || hasWon) return;

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
        Debug.Log("Apunta");
    }

    public void EnableOutline()
    {
        outline.enabled = true;
        Debug.Log("Enciende outline");
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
        Debug.Log(isBeingHeld);
    }

    private void Release()
    {
        isBeingHeld = false;
        puzzleCamera.SetActive(false);
        uiPuzzle.SetActive(false);
        EventManager.Instance.Dispatch(GameEventTypes.OnGameplay, this, EventArgs.Empty);
    }

    [SerializeField] Animator mimicSphere;

    private void OnWinMethod()
    {
        Release();
        canUse = false;
        hasWon = true;
        mimicSphere.SetBool("CanStart", false);
        //SetSphereMaterials(SphereStates.Win);
    }

    public enum SphereStates
    {
        Win,
        Idle,
        Loose
    }

    public void SetSphereMaterials(SphereStates state)
    {
        var renderer = child.GetComponent<MeshRenderer>();
        var materials = renderer.materials;

        Color emissionColor = Color.black;
        Color glassColor;

        switch (state)
        {
            case SphereStates.Win:
                materials[1].EnableKeyword("_EMISSION");
                emissionColor = new Color(1f, 235f / 255f, 0f) * emissionIntensity;
                glassColor = new Color(0f / 255f, 46f / 255f, 191f / 255f, 1f) * emissionIntensity;
                break;
            case SphereStates.Loose:
                materials[1].EnableKeyword("_EMISSION");
                emissionColor = Color.red * emissionIntensity;
                glassColor = Color.red * emissionIntensity;
                break;
            case SphereStates.Idle:
                materials[1].DisableKeyword("_EMISSION");
                glassColor = new Color(0f / 255f, 46f / 255f, 191f / 255f, 1f);
                break;
            default:
                materials[1].EnableKeyword("_EMISSION");
                emissionColor = materials[3].GetColor("_EmissionColor");
                glassColor = new Color(0f / 255f, 46f / 255f, 191f / 255f, 1f) * emissionIntensity;
                break;
        }

        if (state != SphereStates.Idle)
        {
            materials[1].SetColor("_EmissionColor", emissionColor);
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

    private Coroutine fillRoutine;
    [Header("Sphere Settings")]
    [SerializeField] private float rotationSensitivity = 3f;
    [SerializeField] float shaderLerpDuration;

    private void CheckFillAmount()
    {
        float targetFill = 0f;

        switch (fillCounter)
        {
            case 0:
                targetFill = 0f;
                break;
            case 1:
                targetFill = firstFillAmount;
                break;
            case 2:
                targetFill = secondFillAmount;
                break;
            case 3:
                targetFill = thirdFillAmount;
                break;
            default:
                return;
        }

        if (fillRoutine != null)
            StopCoroutine(fillRoutine);

        fillRoutine = StartCoroutine(LerpFillAmount(targetFill));
    }

    private IEnumerator LerpFillAmount(float target)
    {
        float start = fillMat.GetFloat("_FillAmount");
        float speed = 1.4f;

        while (!Mathf.Approximately(start, target))
        {
            start = Mathf.MoveTowards(start, target, speed * 2 *Time.deltaTime);
            fillMat.SetFloat("_FillAmount", start);
            yield return null;
        }

        fillMat.SetFloat("_FillAmount", target);

    }

    private void CheckFrontImage()
    {
        if (!canScore || currentImageIndex >= winImagePoints.Length) return;

        RotateAndCenterCurrentImage();

        Transform currentImage = winImagePoints[currentImageIndex];
        float distance = Vector3.Distance(puzzleCamera.transform.position, currentImage.position);

        Debug.Log($"La distancia de la {currentImage} es de {distance}");

        if (distance < proximityTolerance)
        {
            currentImageIndex++;
            fillCounter++;
            particle.Play();

            if (currentImageIndex >= winImagePoints.Length)
            {
                canScore = false;
                puzzleManager.OnWin();
                OnWinMethod();
            }
        }
        else
        {
            StartCoroutine(LoseInSphere());
        }

        CheckFillAmount();
    }

    public IEnumerator LoseInSphere()
    {
        canUse = false;
        animator.enabled = true;
        anim.SetBool("Lose", true);
        currentImageIndex = 0;
        fillCounter = 0;
        yield return new WaitForSeconds(1f);
        anim.SetBool("Lose", false);
        canUse = true;
        animator.enabled = false;
    }
}