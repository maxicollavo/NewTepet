using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrackerManager : MonoBehaviour
{
    [HideInInspector] public List<Tracker> trackerList = new List<Tracker>();
    public Action<TrackerManager> JeroglificAction;
    public FollowMouseClick trail;

    [SerializeField] GameObject interactor;
    BoxCollider interactorCollider;
    PuzzleInteractor puzzleInteractor;
    public bool OnPuzzle { get; private set; }
    bool HasWon;
    bool canGoBack;
    public bool canInteract;
    public bool subFloor;
    public bool isTarget;
    [SerializeField] GameObject CM_PuzzleCamera;

    [SerializeField] HieroglyficManager hieroglyficManager;

    private void Awake()
    {
        puzzleInteractor = interactor.GetComponent<PuzzleInteractor>();
        interactorCollider = interactor.GetComponent<BoxCollider>();
    }

    private void Start()
    {
        puzzleInteractor.PuzzleAction += OnPuzzleMethod;

        if (hieroglyficManager == null) return;
        hieroglyficManager.OnWinAction += OnWinMethod;
    }

    private void OnWinMethod(HieroglyficManager manager)
    {
        if (!interactorCollider.enabled) return;

        interactorCollider.enabled = false;
    }

    private void Update()
    {
        if (!canGoBack) return;

        if (Input.GetKeyDown(KeyCode.Mouse1) && OnPuzzle)
        {
            BackToGameplay(false);
        }
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

    private void OnPuzzleMethod(PuzzleInteractor interactor)
    {
        if (HasWon) return;
        OnPuzzle = true;
        TurnPuzzleCamera(OnPuzzle);
        interactor.DisableOutline();
        interactorCollider.enabled = false;
        StartCoroutine(EnterPuzzleCoroutine());
    }

    public IEnumerator EnterPuzzleCoroutine()
    {
        canGoBack = false;
        canInteract = false;
        yield return new WaitForSeconds(1.5f);
        canGoBack = true;
        canInteract = true;
        EventManager.Instance.Dispatch(GameEventTypes.OnPuzzle, this, EventArgs.Empty);
    }

    private void BackToGameplay(bool onWin)
    {
        if (HasWon) return;
        OnPuzzle = false;
        TurnPuzzleCamera(OnPuzzle);
        trail.gameObject.SetActive(false);
        EventManager.Instance.Dispatch(GameEventTypes.OnGameplay, this, EventArgs.Empty);
        StartCoroutine(ExitPuzzleCoroutine(onWin));
    }

    public IEnumerator ExitPuzzleCoroutine(bool hasWon)
    {
        canGoBack = false;
        yield return new WaitForSeconds(1.5f);
        canGoBack = true;

        if (!hasWon)
            interactorCollider.enabled = true;
    }

    //Chequea si se ganó el jeroglifico
    public void OnWinMethod()
    {
        if (trackerList.All(t => t.HasWon))
        {
            JeroglificAction?.Invoke(this); //Solo abre la puerta
            interactorCollider.enabled = false; //Desactivamos el collider de ese jeroglifico
            BackToGameplay(true);
            HasWon = true; //Ponemos ese jeroglifico en GANADO

            //Preguntamos si el jeroglifico se encuentra en la parte de abajo y si además es un target
            if (subFloor && isTarget)
            {
                //Si es así entonces chequeamos la victoria
                hieroglyficManager.CheckToUpdateCounter();
            }
        }
    }
}