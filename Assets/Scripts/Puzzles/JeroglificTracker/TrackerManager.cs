using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrackerManager : MonoBehaviour
{
    [HideInInspector] public List<Tracker> trackerList = new List<Tracker>();
    public Action<TrackerManager> JeroglificAction;

    [SerializeField] PuzzleInteractor interactor;
    public bool OnPuzzle { get; private set; }
    bool HasWon;
    public bool subFloor;
    [SerializeField] GameObject CM_PuzzleCamera;
    BoxCollider interactorCollider;

    [SerializeField] HieroglyficManager subFloorManager;

    private void Start()
    {
        interactor.PuzzleAction += OnPuzzleMethod;

        interactorCollider = interactor.GetComponent<BoxCollider>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) && OnPuzzle)
        {
            BackToGameplay();
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
        EventManager.Instance.Dispatch(GameEventTypes.OnPuzzle, this, EventArgs.Empty);
    }

    private void BackToGameplay()
    {
        if (HasWon) return;
        OnPuzzle = false;
        TurnPuzzleCamera(OnPuzzle);
        interactorCollider.enabled = true;
        EventManager.Instance.Dispatch(GameEventTypes.OnGameplay, this, EventArgs.Empty);
    }

    public void OnWinMethod()
    {
        if (trackerList.All(t => t.HasWon))
        {
            Debug.Log("Gana jeroglifico");
            JeroglificAction?.Invoke(this);
            BackToGameplay();
            interactorCollider.enabled = false;
            HasWon = true;

            if (subFloor)
            {
                subFloorManager.CanOpenCeiling();
            }
        }

    }
}