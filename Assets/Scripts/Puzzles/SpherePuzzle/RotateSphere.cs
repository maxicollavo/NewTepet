using System;
using UnityEngine;

public class RotateSphere : MonoBehaviour, Interactor
{
    [Header("Interacción")]
    private Outline outline;
    private SphereCollider coll;
    private bool canUse = true;
    private bool isBeingHeld = false;

    [Header("Rotación")]
    private Transform pivot;
    [SerializeField] private float rotationSensitivity = 3f;

    [Header("Cinemachine")]
    [SerializeField] private GameObject puzzleCamera;

    [Header("OnWin")]
    public Action<RotateSphere> SphereOnWinAction;

    private Vector2 lastMousePos;

    private void Awake()
    {
        outline = GetComponent<Outline>();
        pivot = GetComponent<Transform>();
        coll = GetComponent<SphereCollider>();
    }

    private void Start()
    {
        outline.enabled = false;
    }

    private void Update()
    {
        if (!isBeingHeld) return;

        if (Input.GetMouseButton(0))
        {
            float mouseDelta = Input.GetAxis("Mouse X");
            pivot.Rotate(Vector3.up, -mouseDelta * rotationSensitivity, Space.World);
        }

        if (Input.GetMouseButtonDown(1))
        {
            Release();
        }
    }

    public void Aiming()
    {
        if (!canUse || isBeingHeld) return;

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

        //Al ganar activar objeto de Laser Interactor y apagar este collider

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
        SphereOnWinAction?.Invoke(this);
    }
}
