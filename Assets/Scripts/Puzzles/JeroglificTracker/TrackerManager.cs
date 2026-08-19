using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrackerManager : MonoBehaviour
{
    [HideInInspector] public List<Tracker> trackerList = new List<Tracker>();
    public Action<TrackerManager> HieroglyphCompletedAction;

    [Header("Trail")]
    public FollowMouseClick trail;

    [Header("Interactor")]
    [SerializeField] private GameObject interactor;
    private BoxCollider interactorCollider;
    private PuzzleInteractor puzzleInteractor;

    [Header("State")]
    public bool OnPuzzle { get; private set; }
    public bool HasWon { get; private set; }
    private bool isTransitioning;

    [Header("Flags")]
    public bool isTarget;
    public bool subFloor;

    [Header("Camera")]
    [SerializeField] private GameObject CM_PuzzleCamera;

    [Header("Managers")]
    [SerializeField] private HieroglyficManager hieroglyficManager;
    [SerializeField] private LevelThreeHieroglyficsOnWin levelThreeOnWin;

    private void Awake()
    {
        puzzleInteractor = interactor.GetComponent<PuzzleInteractor>();
        interactorCollider = interactor.GetComponent<BoxCollider>();
    }

    private void Start()
    {
        puzzleInteractor.PuzzleAction += OnPuzzleMethod;

        if (hieroglyficManager != null)
            hieroglyficManager.OnWinAction += OnWinMethod;

        if (trail != null)
            trail.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (puzzleInteractor != null)
            puzzleInteractor.PuzzleAction -= OnPuzzleMethod;

        if (hieroglyficManager != null)
            hieroglyficManager.OnWinAction -= OnWinMethod;
    }

    private void OnWinMethod(HieroglyficManager manager)
    {
        if (interactorCollider == null) return;
        if (!interactorCollider.enabled) return;

        interactorCollider.enabled = false;
    }

    private void Update()
    {
        if (!OnPuzzle || isTransitioning) return;

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Executor(false, null, false);
        }
    }

    public void TurnPuzzleCamera(bool state)
    {
        if (CM_PuzzleCamera != null)
            CM_PuzzleCamera.SetActive(state);
    }

    private void Executor(bool enterPuzzle, PuzzleInteractor interactorArg, bool onWin)
    {
        if (isTransitioning) return;

        if (HasWon && enterPuzzle) return;

        if (enterPuzzle)
        {
            if (interactorArg != null)
                interactorArg.DisableOutline();

            if (interactorCollider != null)
                interactorCollider.enabled = false;

            StartCoroutine(EnterPuzzleCoroutine());
        }
        else
        {
            StartCoroutine(ExitPuzzleCoroutine(onWin));
        }
    }

    private IEnumerator EnterPuzzleCoroutine()
    {
        isTransitioning = true;
        OnPuzzle = true;
        HandsManager.Instance.SetPose(HandPose.Puzzle, ArmTarget.Left);
        NewEventManager.TriggerFreeze(true);

        yield return new WaitForSeconds(.5f);

        TurnPuzzleCamera(true);

        yield return new WaitForSeconds(GameManager.Instance.CameraTransitionTime);

        EventManager.Instance.Dispatch(GameEventTypes.OnPuzzle, this, EventArgs.Empty);
        if (trail != null)
            trail.gameObject.SetActive(true);

        isTransitioning = false;
    }

    private IEnumerator ExitPuzzleCoroutine(bool hasWon)
    {
        isTransitioning = true;

        if (trail != null)
            trail.gameObject.SetActive(false);

        TurnPuzzleCamera(false);

        OnPuzzle = false;

        yield return new WaitForSeconds(GameManager.Instance.CameraTransitionTime);

        HandsManager.Instance.SetPose(HandPose.Gameplay, ArmTarget.Left);
        NewEventManager.TriggerFreeze(false);

        yield return new WaitForSeconds(.5f);

        if (!hasWon && interactorCollider != null)
            interactorCollider.enabled = true;

        EventManager.Instance.Dispatch(GameEventTypes.OnGameplay, this, EventArgs.Empty);

        isTransitioning = false;
    }

    private void OnPuzzleMethod(PuzzleInteractor interactorArg)
    {
        Executor(true, interactorArg, false);
    }

    public void OnWinMethod()
    {
        if (HasWon) return;

        if (trackerList.All(t => t.HasWon))
        {
            HieroglyphCompletedAction?.Invoke(this);

            Executor(false, null, true);

            HasWon = true;

            if (interactorCollider != null)
                interactorCollider.enabled = false;

            if (levelThreeOnWin != null)
                levelThreeOnWin.CheckToUpdateCounter();

            if (subFloor && isTarget && hieroglyficManager != null)
                hieroglyficManager.CheckToUpdateCounter();
        }
    }
}
