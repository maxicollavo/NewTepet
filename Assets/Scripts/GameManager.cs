using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

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

    public bool BlendedToPlayer = true;

    public bool HasPiece;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        brain = Camera.main.GetComponent<CinemachineBrain>();
    }

    private void Start()
    {
        EventManager.Instance.Register(GameEventTypes.OnCinematic, OnCinematicMethod);
        EventManager.Instance.Register(GameEventTypes.OnGameplay, OnGameplayMethod);
        EventManager.Instance.Register(GameEventTypes.OnPuzzle, OnPuzzleMethod);

        EventManager.Instance.Dispatch(GameEventTypes.OnGameplay, this, EventArgs.Empty);
    }

    private void OnDestroy()
    {
        EventManager.Instance.Unregister(GameEventTypes.OnCinematic, OnCinematicMethod);
        EventManager.Instance.Unregister(GameEventTypes.OnGameplay, OnGameplayMethod);
        EventManager.Instance.Unregister(GameEventTypes.OnPuzzle, OnPuzzleMethod);
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

    public void OnCinematicMethod(object sender, EventArgs e)
    {
        Debug.Log("Llama a OnCinematic");
        SetGameplayElementsActive(false);
        FPController.enabled = false;
    }

    public void OnPuzzleMethod(object sender, EventArgs e)
    {
        Debug.Log("Llama a OnPuzzle");
        SetGameplayElementsActive(false);
        FPController.enabled = false;
    }

    public void OnGameplayMethod(object sender, EventArgs e)
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