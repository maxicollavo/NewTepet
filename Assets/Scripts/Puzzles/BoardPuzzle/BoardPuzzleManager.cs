using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class BoardPuzzleManager : MonoBehaviour
{
    [Header("Callbacks")]
    public Action<BoardPuzzleManager> OnWin;

    [Header("Piece Movement")]
    public float moveSpeed;
    bool isMoving;

    [Header("Pieces")]
    public BoardPiece[] pieces;
    private BoardPiece selectedPiece;
    public GameObject pieceGo;
    public GameObject handPiece;

    [Header("Waypoints")]
    private BoardWaypoint currentWp;
    private BoardWaypoint newWaypoint;
    [SerializeField] private BoardWaypoint[] targets;

    [Header("Buttons")]
    public BoardButton[] buttons;
    private BoardButton pressedButton;
    private Vector2 direction;
    [SerializeField] MovePieceASDW movePiece;

    [Header("Colliders")]
    [SerializeField] List<BoxCollider> colliders;
    private bool previousState;

    [Header("Settings")]
    [SerializeField] BoxCollider interactorCollider;
    [SerializeField] GameObject CM_PuzzleCamera;
    public bool OnPuzzle { get; private set; }
    private bool HasPiece;
    private bool HasWon;
    private bool canInteract = false;
    private bool canGoBack;

    [SerializeField] ObjectsToPick requiredObj;

    private Dictionary<BoardPiece, BoardWaypoint> pieceTargetMap = new Dictionary<BoardPiece, BoardWaypoint>();

    [SerializeField] PuzzleInteractor interactor;

    private void Start()
    {
        movePiece.OnButtonPressed += GetButtonPressed;
        foreach (var p in pieces)
        {
            p.OnPieceSelected += GetSelectedPiece;
        }

        foreach (var b in buttons)
        {
            b.OnButtonPressed += GetButtonPressed;
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
        movePiece.OnButtonPressed -= GetButtonPressed;
        foreach (var p in pieces) p.OnPieceSelected -= GetSelectedPiece;
        foreach (var b in buttons) b.OnButtonPressed -= GetButtonPressed;
        interactor.PuzzleAction -= OnPuzzleMethod;
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

    void OnPuzzleMethod(PuzzleInteractor interactor)
    {
        SetOnPuzzle();
    }

    private void SetOnPuzzle()
    {
        if (HasWon) return;

        if (PickedObjData.Instance.WasPicked(requiredObj))
        {
            HasPiece = true;
            pieceGo.SetActive(true);
            handPiece.SetActive(false);
            PickedObjData.Instance.MarkAsThrowed(requiredObj);
            return;
        }

        if (!HasPiece)
        {
            StartCoroutine(CannotEnter(interactor));
            return;
        }

        movePiece.enabled = true;
        interactor.DisableOutline();
        interactorCollider.enabled = false;
        OnPuzzle = true;
        TurnPuzzleCamera(OnPuzzle);
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

        if (currentWp.neighbors.TryGetValue(direction, out BoardWaypoint nextWp) && !nextWp.IsUsing)
        {
            newWaypoint = nextWp;
            StartCoroutine(MoveToTarget(selectedPiece, newWaypoint.transform.position, currentWp, nextWp, button));
        }
        else
        {
            StartCoroutine(CannotMove(button));
        }
    }


    void GetSelectedPiece(BoardPiece piece)
    {
        if (selectedPiece != null)
        {
            selectedPiece.DeselectPiece();
            selectedPiece = null;
        }

        selectedPiece = piece;
        currentWp = piece.currentWp;
    }

    private IEnumerator MoveToTarget(BoardPiece piece, Vector3 targetPos, BoardWaypoint currentW, BoardWaypoint nextW, IButtonInput button)
    {
        if (button is IOutlineButton ob)
        {
            ob.EnableOutline();
        }

        SetPiecesTrigger(false);

        while (Vector3.Distance(piece.transform.position, targetPos) > 0.01f)
        {
            piece.transform.position = Vector3.MoveTowards(piece.transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        piece.transform.position = targetPos;
        yield return new WaitForSeconds(0.2f);

        currentW.IsUsing = false;
        nextW.IsUsing = true;
        AudioManager.Instance.PlaySound("MovePieceBoard");
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
        foreach (BoardPiece piece in pieces)
        {
            piece.coll.enabled = state;
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

    private IEnumerator CannotEnter(PuzzleInteractor button)
    {
        button.EnableOutline();
        button.outline.OutlineColor = Color.red;
        yield return new WaitForSeconds(0.5f);
        button.outline.OutlineColor = Color.white;
        button.DisableOutline();
        isMoving = false;
    }

    private void CheckPosition(BoardPiece piece, BoardWaypoint wp)
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