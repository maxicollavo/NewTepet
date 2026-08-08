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
    [SerializeField] private GameObject _buttonGo;

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
    private Coroutine _executeRoutine;
    private bool _isTransitioning;
    private void EnterDialMethod()
    {
        if (hasWon) return;
        Execute(true, GameEventTypes.OnPuzzle);
    }

    private void ExitDialMethod()
    {
        Execute(false, GameEventTypes.OnGameplay);
    }

    private void Execute(bool state, GameEventTypes eventType)
    {
        if (state == _onPuzzle) return;

        if (_isTransitioning) return;

        _executeRoutine = StartCoroutine(ExecuteCoroutine(state, eventType));
    }

    private IEnumerator ExecuteCoroutine(bool state, GameEventTypes eventType)
    {
        _isTransitioning = true;

        if (state)
        {
            NewEventManager.TriggerFreeze(true);
            HandsManager.Instance.SetPose(HandPose.Puzzle, ArmTarget.Both);

            GameplayStates(true);

            yield return new WaitForSeconds(0.5f);
        }

        TurnDialCamera(state);
        RotatableDialState(state);
        TurnUI(state);

        if (!state)
        {
            yield return new WaitForSeconds(GameManager.Instance.CameraTransitionTime);
            HandsManager.Instance.SetPose(HandPose.Gameplay, ArmTarget.Both);
            NewEventManager.TriggerFreeze(false);

            GameplayStates(false);
        }

        EventManager.Instance.Dispatch(eventType, this, EventArgs.Empty);

        _isTransitioning = false;
        _executeRoutine = null;
    }

    private void TurnUI(bool state)
    {
        foreach (var e in _dialElements)
        {
            e.SetActive(state);
        }
    }
    #endregion

    #region OnWin
    [SerializeField] Animator _doorAnim;
    [SerializeField] AudioSource _doorSound;
    private void OnWinMethod()
    {
        Reward();
    }

    private void Reward()
    {
        ExitDialMethod();
        hasWon = true;
        _buttonGo.GetComponent<BoxCollider>().enabled = false;
        StartCoroutine(RewardCoroutine());
    }

    private IEnumerator RewardCoroutine()
    {
        yield return new WaitForSeconds(1.5f);
        _doorAnim.SetTrigger("Open");
        _doorSound.Play();
        _buttonGo.GetComponent<Animator>().SetTrigger("Interact");
    }
    #endregion
}
