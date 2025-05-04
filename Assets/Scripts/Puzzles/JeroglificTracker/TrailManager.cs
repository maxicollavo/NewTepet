using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrailManager : MonoBehaviour
{
    [Header("Start Settings")]
    public bool CanStart;
    [SerializeField] StatueManager statueManager;
    [SerializeField] List<GameObject> nodesToEnable;

    [Header("Nodes")]
    [SerializeField] List<GameObject> actionNodes; //Referencia a su action
    [SerializeField] List<GameObject> validNodes; //Los nodos que tengo que tocar
    private List<GameObject> currentPath = new List<GameObject>(); //Los nodos que toco
    [SerializeField] private LayerMask nodeLayerMask;

    [Header("Particle")]
    [SerializeField] ParticleSystem particle1;
    [SerializeField] ParticleSystem particle2;

    [Header("Tracker")]
    public bool isTracking { get; private set; }
    [SerializeField] GameObject tracker1;
    [SerializeField] GameObject tracker2;

    [Header("Settings")]
    private Camera playerCam;
    [SerializeField] GameObject playerHand;
    [SerializeField] GameObject CM_PuzzleCamera;
    [SerializeField] PuzzleInteractor interactor;
    private bool previousState = false;
    public bool OnPuzzle { get; private set; }
    [SerializeField] BoxCollider interactorCollider;

    [Header("On Win")]
    public Action<TrailManager> JeroglificAction;
    private bool HasWon;

    private void Start()
    {
        interactor.PuzzleAction += OnPuzzleMethod;
        playerCam = Camera.main;

        if (statueManager == null) return;
        statueManager.StatueManagerAction += OnStatueFinish;
    }

    private void OnStatueFinish(StatueManager manager)
    {
        CanStart = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) && OnPuzzle)
        {
            BackToGameplay();
        }

        if (HasWon || !CanStart) return;

        bool currentState = OnPuzzle;

        if (currentState != previousState)
        {
            tracker1.SetActive(currentState);
            tracker2.SetActive(currentState);
            previousState = currentState;
        }

        if (!currentState) return;
        Debug.Log(currentState);

        if (Input.GetMouseButtonDown(0))
        {
            currentPath.Clear();
            isTracking = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isTracking = false;
            RestartTracking();
        }

        if (isTracking)
        {
            TrackMouse();
        }

        ParticleTracking(isTracking);
    }

    void OnPuzzleMethod(PuzzleInteractor interactor)
    {
        OnPuzzle = true;
        playerHand.SetActive(false);
        interactor.DisableOutline();
        TurnPuzzleCamera(OnPuzzle);
        interactorCollider.enabled = false;
        EventManager.Instance.Dispatch(GameEventTypes.OnPuzzle, this, EventArgs.Empty);
    }

    public void BackToGameplay()
    {
        OnPuzzle = false;
        playerHand.SetActive(true);
        TurnPuzzleCamera(OnPuzzle);
        EventManager.Instance.Dispatch(GameEventTypes.OnGameplay, this, EventArgs.Empty);
        if (HasWon) return;

        interactorCollider.enabled = true;
    }

    private void TurnPuzzleCamera(bool state)
    {
        if (state)
        {
            CM_PuzzleCamera.SetActive(true);
        }
        else
        {
            CM_PuzzleCamera.SetActive(false);
        }
    }

    void TrackMouse()
    {
        Ray ray = playerCam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, nodeLayerMask))
        {
            GameObject hitObj = hit.collider.gameObject;

            CheckIfValid(hitObj);
        }
        else
        {
            RestartTracking();
        }
    }

    void RestartTracking()
    {
        isTracking = false;
        currentPath.Clear();
        Debug.Log(currentPath.Count);
    }

    void CheckIfValid(GameObject node)
    {
        if (validNodes.Contains(node) && !currentPath.Contains(node))
        {
            AddNode(node);
            CheckWin();
        }
        {
            Debug.Log(currentPath.Count);
        }
    }

    void CheckWin()
    {
        if (currentPath.Count == validNodes.Count)
        {
            isTracking = false;
            Win();
        }
    }

    void Win()
    {
        JeroglificAction?.Invoke(this);
        HasWon = true;
        BackToGameplay();
    }

    void AddNode(GameObject node)
    {
        currentPath.Add(node);
    }

    private void ParticleTracking(bool isTracking)
    {
        if (isTracking)
        {
            if (!particle1.isPlaying)
            {
                particle1.Play();
                particle2.Play();
            }
        }
        else
        {
            if (particle1.isPlaying)
            {
                particle1.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                particle2.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }
    }
}