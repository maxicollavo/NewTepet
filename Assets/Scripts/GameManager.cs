using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class GameManager : MonoBehaviour
{
    public bool clickBlock;

    [Header("States Manager")]
    [HideInInspector] public PowerStates state;

    [Header("Pause Manager")]
    [SerializeField] GameObject pauseMenu;
    private bool menuPressed;

    [Header("Gameplay")]
    public List<GameObject> TPWaypoints;
    public FPSController FPController;
    public GameObject hand;
    public GameObject crosshair;
    public CameraManager camManager;
    public CinemachineCamera playerCam;
    public bool canCheck;
    private bool requiresHand;

    public bool HasPiece;

    [SerializeField] Animator pyramidOnEnterAnim;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        CinemachineCore.BlendCreatedEvent.AddListener(OnBlendCreated);
        CinemachineCore.BlendFinishedEvent.AddListener(OnBlendFinished);

        EventManager.Instance.Dispatch(GameEventTypes.OnGameplay, this, EventArgs.Empty);
    }

    private void OnDestroy()
    {
        CinemachineCore.BlendCreatedEvent.RemoveListener(OnBlendCreated);
        CinemachineCore.BlendFinishedEvent.RemoveListener(OnBlendFinished);
    }

    private void OnBlendCreated(CinemachineCore.BlendEventParams evtParams)
    {
        if (evtParams.Blend.CamA == playerCam as ICinemachineCamera)
        {
            var camA = evtParams.Blend.CamB as CinemachineCamera;

            if (camA != null)
            {
                GameObject camObject = camA.gameObject;
                PuzzleDefiner definer = camObject.GetComponent<PuzzleDefiner>();

                if (definer != null)
                {
                    requiresHand = definer.requiresHand;
                    OnPuzzleMethod(requiresHand);
                    return;
                }
            }

            OnPuzzleMethod(false);
        }
    }

    private void OnBlendFinished(ICinemachineMixer cam1, ICinemachineCamera cam2)
    {
        if (cam2 == playerCam as ICinemachineCamera)
        {
            OnGameplayMethod();
        }
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseTrigger();
        }
    }

    private void PauseTrigger()
    {
        menuPressed = !menuPressed;
        pauseMenu.SetActive(menuPressed);

        if (menuPressed)
        {
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void OnPuzzleMethod(bool requiresHand)
    {
        SetGameplayElementsActive(false, requiresHand);
        FPController.enabled = false;
    }

    void OnGameplayMethod()
    {
        SetGameplayElementsActive(true, true);
        FPController.enabled = true;
        EventManager.Instance.Dispatch(GameEventTypes.OnGameplay, this, EventArgs.Empty);
    }

    private void SetGameplayElementsActive(bool active, bool requiresHand)
    {
        crosshair.SetActive(active);
        hand.SetActive(requiresHand);

        if (requiresHand)
        {
            pyramidOnEnterAnim.SetTrigger("EnterPuzzle");
        }
    }
}

public enum PowerStates
{
    OnLaser,
    OnDimension
}

public enum RailColors
{
    Red,
    Blue,
    Yellow,
    Green
}