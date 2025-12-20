using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

public class TrackerManager : MonoBehaviour
{
    [HideInInspector] public List<Tracker> trackerList = new List<Tracker>();
    public Action<TrackerManager> HieroglyphCompletedAction;
    public FollowMouseClick trail;
    [SerializeField] GameObject interactor;
    BoxCollider interactorCollider;
    PuzzleInteractor puzzleInteractor;
    public bool OnPuzzle { get; private set; }
    public bool HasWon;
    bool canGoBack;
    public bool canInteract;
    public bool subFloor;
    public bool isTarget;
    [SerializeField] GameObject CM_PuzzleCamera;
    [SerializeField] HieroglyficManager hieroglyficManager;
    [SerializeField] LevelThreeHieroglyficsOnWin levelThreeOnWin;

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

    public void TurnPuzzleCamera(bool state)
    {
        CM_PuzzleCamera.SetActive(state);
    }

    private void OnPuzzleMethod(PuzzleInteractor interactor)
    {
        if (HasWon) return;

        OnPuzzle = true;
        TurnPuzzleCamera(true);
        interactor.DisableOutline();
        interactorCollider.enabled = false;
        StartCoroutine(EnterPuzzleCoroutine());
    }

    public IEnumerator EnterPuzzleCoroutine()
    {
        canGoBack = false;
        canInteract = false;
        yield return new WaitForSeconds(1.3f);
        HandsManager.Instance.SetPose(HandPose.Hieroglyfic, ArmTarget.Left);
        yield return new WaitForSeconds(.5f);
        canGoBack = true;
        canInteract = true;
        EventManager.Instance.Dispatch(GameEventTypes.OnPuzzle, this, EventArgs.Empty);
    }

    public void BackToGameplay(bool onWin)
    {
        if (HasWon) return;

        OnPuzzle = false;
        trail.gameObject.SetActive(false);
        EventManager.Instance.Dispatch(GameEventTypes.OnGameplay, this, EventArgs.Empty);
        StartCoroutine(ExitPuzzleCoroutine(onWin));
    }

    public IEnumerator ExitPuzzleCoroutine(bool hasWon)
    {
        yield return new WaitForSeconds(0.3f);
        TurnPuzzleCamera(false);
        canGoBack = false;
        yield return new WaitForSeconds(1f);
        HandsManager.Instance.SetPose(HandPose.Gameplay, ArmTarget.Left);
        yield return new WaitForSeconds(.5f);
        canGoBack = true;

        if (!hasWon)
            interactorCollider.enabled = true;
    }
    // Chequea si se ganó el jeroglífico
    public void OnWinMethod()
    {
        if (trackerList.All(t => t.HasWon))
        {
            HieroglyphCompletedAction?.Invoke(this);
            WinPuzzle();
            interactorCollider.enabled = false; // Desactiva el collider
            Debug.Log($"Se desactivo el collider del objeto {this.gameObject}");
            if (levelThreeOnWin != null)
                levelThreeOnWin.CheckToUpdateCounter();

            if (subFloor && isTarget)
            {
                if (hieroglyficManager != null)
                    hieroglyficManager.CheckToUpdateCounter();
            }
        }
    }

    private void WinPuzzle()
    {
        trail.gameObject.SetActive(false); //Apagamos el rayo
        TurnPuzzleCamera(false);
        BackToGameplay(true); //Volvemos a la camara de gameplay
        HasWon = true;
    }
}
