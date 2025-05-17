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
    private CinemachineBrain brain;
    [SerializeField] CinemachineCamera playerCam;
    public bool canCheck;

    public bool HasPiece;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        brain = Camera.main.GetComponent<CinemachineBrain>();
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
        if (evtParams.Blend.CamA == playerCam)
        {
            OnPuzzleMethod();
        }
    }

    private void OnBlendFinished(ICinemachineMixer cam1, ICinemachineCamera cam2)
    {
        if (cam2 == playerCam)
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

    void OnCinematicMethod()
    {
        Debug.Log("Llama a OnCinematic");
        SetGameplayElementsActive(false);
        FPController.enabled = false;
    }

    void OnPuzzleMethod()
    {
        Debug.Log("Llama a OnPuzzle");
        SetGameplayElementsActive(false);
        FPController.enabled = false;
    }

    void OnGameplayMethod()
    {
        Debug.Log("Llama a OnGameplay");
        SetGameplayElementsActive(true);
        FPController.enabled = true;
    }

    private void SetGameplayElementsActive(bool active)
    {
        hand.SetActive(active);
        crosshair.SetActive(active);
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