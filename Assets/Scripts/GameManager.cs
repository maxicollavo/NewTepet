using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("States Manager")]
    [HideInInspector] public PowerStates state;

    [Header("Pause Manager")]
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject CurrentPowerUI;
    private bool pauseTriggered;

    [Header("Gameplay")]
    public List<GameObject> TPWaypoints;
    public FPSController FPController;
    public GameObject hand;
    public GameObject crosshair;
    public CameraManager camManager;
    public CinemachineCamera playerCam;
    public bool canCheck;
    private bool requiresHand;
    [SerializeField] GameObject playerObj;
    [SerializeField] Transform playerSpawner;

    public bool HasPiece;

    [SerializeField] Animator pyramidOnEnterAnim;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SpawnPlayer();

        CinemachineCore.BlendCreatedEvent.AddListener(OnBlendCreated);
        CinemachineCore.BlendFinishedEvent.AddListener(OnBlendFinished);

        EventManager.Instance.Dispatch(GameEventTypes.OnGameplay, this, EventArgs.Empty);

        Time.timeScale = 1;
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

        if (Input.GetKeyDown(KeyCode.F1))
        {
            SceneManager.LoadScene("Level_One");
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            SceneManager.LoadScene("Level_Two");
        }
    }

    private void SpawnPlayer()
    {
        playerObj.transform.position = playerSpawner.position;
        playerObj.transform.rotation = playerSpawner.rotation;
    }

    private void PauseTrigger()
    {
        pauseTriggered = !pauseTriggered;
        NewEventManager.TriggerPause(pauseTriggered);
        PlayerInputHandler.Instance.EnableLook(!pauseTriggered);
        pauseMenu.SetActive(pauseTriggered);

        if (pauseTriggered)
        {
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            CurrentPowerUI.SetActive(false);
        }
        else
        {
            Time.timeScale = 1;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            CurrentPowerUI.SetActive(true);
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