using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverPuzzleManager : MonoBehaviour
{
    [Header("Callbacks")]
    public Action<RiverPuzzleManager> OnWin;

    [Header("Piece Movement")]
    public float moveSpeed;
    bool isMoving;

    [Header("Pieces")]
    public RiverPiece[] pieces;
    private RiverPiece selectedPiece;

    [Header("Waypoints")]
    private RiverWaypoint currentWp;
    private RiverWaypoint newWaypoint;
    [SerializeField] private RiverWaypoint[] targets;

    [Header("Movement Inputs")]
    private Vector2 direction;
    [SerializeField] MovePieceASDW movePiece;

    [Header("Colliders")]
    List<BoxCollider> colliders = new List<BoxCollider>();
    private bool previousState;

    [Header("Settings")]
    [SerializeField] GameObject CM_PuzzleCamera;
    public bool OnPuzzle { get; private set; }
    private bool HasWon;
    private bool canInteract = false;
    private bool canGoBack;
    //public GameObject uiRiverPuzzle;
    private Dictionary<RiverPiece, RiverWaypoint> pieceTargetMap = new Dictionary<RiverPiece, RiverWaypoint>();
    [SerializeField] GameObject interactorGO;
    BoxCollider interactorCollider;
    PuzzleInteractor interactor;
    PuzzleDefiner definer;

    private void Awake()
    {
        foreach (var piece in pieces)
        {
            colliders.Add(piece.gameObject.GetComponent<BoxCollider>());
        }

        interactorCollider = interactorGO.GetComponent<BoxCollider>();
        interactor = interactorGO.GetComponent<PuzzleInteractor>();
        definer = CM_PuzzleCamera.GetComponent<PuzzleDefiner>();
    }

    private void Start()
    {
        movePiece.OnButtonPressed += GetButtonPressed;
        foreach (var p in pieces)
        {
            p.OnPieceSelected += GetSelectedPiece;
        }


        for (int i = 0; i < pieces.Length; i++)
        {
            pieceTargetMap[pieces[i]] = targets[i];
        }

        foreach (var c in colliders)
        {
            c.enabled = false;
        }

        interactor.PuzzleAction += OnPuzzleMethod;
    }

    private void OnDestroy()
    {
        foreach (var p in pieces) p.OnPieceSelected -= GetSelectedPiece;
        interactor.PuzzleAction -= OnPuzzleMethod;
        movePiece.OnButtonPressed -= GetButtonPressed;
    }

    private void Update()
    {
        bool currentState = OnPuzzle;

        if (currentState != previousState)
        {
            SwitchColliders(currentState);
            previousState = currentState;
        }


        if (!canGoBack) return;

        if (currentState && Input.GetKeyDown(KeyCode.Mouse1))
        {
            BackToGameplay();
        }

    }

    void GetButtonPressed(Vector2 pressedDirection, IButtonInput button)
    {
        if (!canInteract || isMoving) return;

        isMoving = true;
        direction = pressedDirection;

        if (selectedPiece == null || currentWp == null)
        {
            isMoving = false;
            return;
        }

        if (currentWp.neighbors.TryGetValue(direction, out RiverWaypoint nextWp) && !nextWp.IsUsing)
        {
            newWaypoint = nextWp;
            StartCoroutine(MoveToTarget(selectedPiece, newWaypoint.transform.position, currentWp, nextWp, button));
        }
        else
        {
            StartCoroutine(CannotMove(button));
        }
    }

    private IEnumerator CannotMove(IButtonInput button)
    {
        if (button is IOutlineButton ob)
        {
            ob.EnableOutline();
            var originalColor = ob.outline.OutlineColor;
            ob.outline.OutlineColor = Color.red;
            yield return new WaitForSeconds(0.5f);
            ob.outline.OutlineColor = originalColor;
            ob.DisableOutline();
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }

        isMoving = false;
    }

    void OnPuzzleMethod(PuzzleInteractor interactor)
    {
        EnterPuzzle();
    }

    public void EnterPuzzle()
    {
        if (HasWon) return;

        StartCoroutine(EnterPuzzleCoroutine());
    }

    public IEnumerator EnterPuzzleCoroutine()
    {
        TurnPuzzleCamera(true);
        interactor.DisableOutline();
        interactorCollider.enabled = false;
        canGoBack = false;
        canInteract = false;
        yield return new WaitForSeconds(1.5f);
        canGoBack = true;
        canInteract = true;
        EventManager.Instance.Dispatch(GameEventTypes.OnPuzzle, this, EventArgs.Empty);
        CanInteractWithPuzzle();
    }

    public void CanInteractWithPuzzle()
    {
        movePiece.enabled = true;
        OnPuzzle = true;
        //if (uiRiverPuzzle != null)
        //    uiRiverPuzzle.SetActive(true);
    }

    public void BackToGameplay()
    {
        SetOnGameplay();
    }

    private void SetOnGameplay()
    {
        if (selectedPiece != null)
        {
            selectedPiece.DeselectPiece();
            selectedPiece = null;
        }

        //if (uiRiverPuzzle != null)
        //    uiRiverPuzzle.SetActive(false);

        movePiece.enabled = false;
        OnPuzzle = false;
        TurnPuzzleCamera(OnPuzzle);
        EventManager.Instance.Dispatch(GameEventTypes.OnGameplay, this, EventArgs.Empty);
        StartCoroutine(ExitPuzzleCoroutine());
    }

    public IEnumerator ExitPuzzleCoroutine()
    {
        canGoBack = false;
        yield return new WaitForSeconds(1.5f);
        canGoBack = true;

        if (!HasWon)
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

    void SwitchColliders(bool state)
    {
        foreach (var c in colliders)
        {
            c.enabled = state;
        }
    }

    void GetSelectedPiece(RiverPiece piece)
    {
        if (selectedPiece != null)
        {
            selectedPiece.DeselectPiece();
            selectedPiece = null;
        }

        selectedPiece = piece;
        currentWp = piece.currentWp;
    }

    private IEnumerator MoveToTarget(RiverPiece piece, Vector3 targetPos, RiverWaypoint currentW, RiverWaypoint nextW, IButtonInput button)
    {
        SetPiecesTrigger(false);
        AudioManager.Instance.PlaySound("MoveStone2");

        while (Vector3.Distance(piece.transform.position, targetPos) > 0.01f)
        {
            piece.transform.position = Vector3.MoveTowards(piece.transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        piece.transform.position = targetPos;
        yield return new WaitForSeconds(0.2f);

        currentW.IsUsing = false;
        nextW.IsUsing = true;
        piece.currentWp = nextW;
        currentWp = nextW;
        isMoving = false;
        SetPiecesTrigger(true);

        if (button is IOutlineButton obEnd)
        {
            obEnd.DisableOutline();
        }

        CheckPosition(piece, nextW);
    }

    private void SetPiecesTrigger(bool state)
    {
        foreach (RiverPiece piece in pieces)
        {
            piece.coll.enabled = state;
        }
    }

    private void CheckPosition(RiverPiece piece, RiverWaypoint wp)
    {
        if (!pieceTargetMap.ContainsKey(piece)) return;

        var targetWp = pieceTargetMap[piece];

        if (wp == targetWp)
        {
            piece.OnPositionWinner = true;
        }
        else
        {
            piece.OnPositionWinner = false;
        }

        CheckWin();
    }

    private void CheckWin()
    {
        foreach (var piece in pieces)
        {
            if (!piece.OnPositionWinner) return;
        }

        Win();
    }

    void Win()
    {
        HasWon = true;
        BackToGameplay();
        OnWin?.Invoke(this);
        foreach (var piece in pieces) piece.DisableOutline();
    }
}