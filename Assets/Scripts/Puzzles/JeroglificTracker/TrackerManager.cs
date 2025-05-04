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
    [SerializeField] GameObject CM_PuzzleCamera;
    BoxCollider interactorCollider;

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
        OnPuzzle = true;
        TurnPuzzleCamera(OnPuzzle);
        interactor.DisableOutline();
        interactorCollider.enabled = false;
        EventManager.Instance.Dispatch(GameEventTypes.OnPuzzle, this, EventArgs.Empty);
    }

    private void BackToGameplay()
    {
        OnPuzzle = false;
        TurnPuzzleCamera(OnPuzzle);

        if (HasWon) return;
        interactorCollider.enabled = true;
        EventManager.Instance.Dispatch(GameEventTypes.OnGameplay, this, EventArgs.Empty);
    }

    public void OnWinMethod()
    {
        if (trackerList.All(t => t.HasWon))
        {
            Debug.Log("Gana jeroglifico");
            JeroglificAction?.Invoke(this);
            HasWon = true;
            BackToGameplay();
        }
    }
}