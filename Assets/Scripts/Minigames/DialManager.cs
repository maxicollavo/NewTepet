using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialManager : MonoBehaviour
{
    [SerializeField] CheckAlignmentsDial checkAlignments;
    [SerializeField] EnterDial enterDial;

    [SerializeField] List<GameObject> _dialElements;

    public Action OnSpaceAction;
    public static DialManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        enterDial.EnterDialAction += EnterDialMethod;
        checkAlignments.OnDialAligned += OnWinMethod;

        _rotatableDialArray = checkAlignments.GetDials();
    }

    private void Update()
    {
        if (!_canGoBack || hasWon) return;

        if (Input.GetKeyDown(KeyCode.Mouse1) && _onPuzzle)
        {
            ExitDialMethod();
        }

        if (Input.GetKeyDown(KeyCode.Space) && _onPuzzle)
        {
            OnSpaceAction?.Invoke();
        }
    }

    #region States
    RotatableDial[] _rotatableDialArray;
    private bool _onPuzzle;
    private bool _canGoBack;
    private bool hasWon;

    private void RotatableDialState(bool state)
    {
        foreach (var d in _rotatableDialArray)
        {
            d.canRotateDial = state;
        }
    }

    private void GameplayStates(bool state)
    {
        _onPuzzle = state;
        _canGoBack = state;
    }
    #endregion

    #region Cameras
    [SerializeField] GameObject _dialCamera;

    public void TurnDialCamera(bool state)
    {
        _dialCamera.SetActive(state);
    }
    #endregion

    #region Enter and Exit
    private void EnterDialMethod()
    {
        if (hasWon) return;
        TurnDialCamera(true);
        RotatableDialState(true);
        GameplayStates(true);
        TurnUI(true);

        EventManager.Instance.Dispatch(GameEventTypes.OnPuzzle, this, EventArgs.Empty);
    }

    private void TurnUI(bool state)
    {
        foreach(var e in _dialElements)
        {
            e.SetActive(state);
        }
    }

    private void ExitDialMethod()
    {
        TurnDialCamera(false);
        RotatableDialState(false);
        GameplayStates(false);
        TurnUI(false);

        EventManager.Instance.Dispatch(GameEventTypes.OnGameplay, this, EventArgs.Empty);
    }
    #endregion

    #region OnWin
    [SerializeField] Animator _doorAnim;
    [SerializeField] AudioSource _doorSound;
    private void OnWinMethod()
    {
        ExitDialMethod();
        //Reward();
        hasWon = true;
        _doorAnim.SetTrigger("Open");
        //_doorSound.Play();
    }
    #endregion
}
