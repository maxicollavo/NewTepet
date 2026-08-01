using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColumnInteractManager : MonoBehaviour
{
    [HideInInspector]
    public Dictionary<InteriorPieceSelector, bool> interiorPieceSelected = new Dictionary<InteriorPieceSelector, bool>();
    public Action<ColumnInteractManager> OnWinAction;

    [SerializeField] float rotationSpeed = 50f;
    [SerializeField] float alignThreshold;
    [SerializeField] float winThreshold;
    [SerializeField] EnterColumnPuzzle enterPuzzle;

    private ColumnSelected currentlySelected;
    private Transform forward;
    private Transform lookAtTarget;

    public bool canRotate;
    public bool isRotating { get; private set; }
    [HideInInspector] public bool hasWon;

    private int piecesCounter;
    private int oldPieceCounter;

    [Header("Columnas")]
    [SerializeField] private List<ColumnSelected> allColumns = new List<ColumnSelected>();
    private int currentColumnIndex = 0;

    [Header("Camera Shake")]
    [SerializeField] private CameraShake cameraShake;
    [SerializeField] private float shakeMagnitude;

    [Header("VFX Alpha")]
    [SerializeField] private float alphaLerpSpeed;
    [SerializeField] private float targetAlpha;
    private float currentAlpha = 0f;

    private InteriorPieceSelector previousPiece;
    private List<InteriorPieceSelector> fadingPieces = new List<InteriorPieceSelector>();
    private InteriorPieceSelector rotatingPiece;

    [SerializeField] private float rotationStep = 20f;
    [SerializeField] private float rotationDuration = 0.15f;

    public void OnSelectedMethod(int columnIndex, bool isSelected, ColumnSelected selected)
    {
        currentColumnIndex = columnIndex;

        if (currentlySelected != null && currentlySelected != selected)
        {
            oldPieceCounter = piecesCounter;

            if (currentlySelected.interiorPieces.Length > piecesCounter)
                currentlySelected.interiorPieces[piecesCounter].DisableOutline();

            currentlySelected.DeselectPiece();
        }

        if (isSelected)
        {
            currentlySelected = selected;

            oldPieceCounter = piecesCounter;
            piecesCounter = oldPieceCounter;

            forward = currentlySelected.interiorPieces[piecesCounter].forward;
            lookAtTarget = currentlySelected.interiorPieces[piecesCounter].lookAtTarget;

            if (currentlySelected.interiorPieces.Length > 0)
                currentlySelected.interiorPieces[piecesCounter].EnableOutline();
        }
        else
        {
            currentlySelected = null;
            forward = null;
            lookAtTarget = null;
        }
    }

    private void Update()
    {
        if (!canRotate) return;

        float target = isRotating ? targetAlpha : 0f;
        currentAlpha = Mathf.MoveTowards(currentAlpha, target, alphaLerpSpeed * Time.deltaTime);
        ApplyVFXAlpha(currentAlpha);

        if (fadingPieces.Count > 0)
        {
            for (int i = fadingPieces.Count - 1; i >= 0; i--)
            {
                var p = fadingPieces[i];
                bool finished = FadeOutPiece(p);
                if (finished)
                    fadingPieces.RemoveAt(i);
            }
        }

        if (currentlySelected != null && !currentlySelected.interiorPieces[piecesCounter].hasWon)
        {
            Transform columnTransform = currentlySelected.interiorPieces[piecesCounter].columnTransform;
            Transform tableColumnTransform = currentlySelected.interiorPieces[piecesCounter].transform.parent.transform;

            if (!isRotating)
            {
                if (Input.GetKeyDown(KeyCode.A))
                    StartCoroutine(RotatePiece(columnTransform, tableColumnTransform, rotationStep));

                if (Input.GetKeyDown(KeyCode.D))
                    StartCoroutine(RotatePiece(columnTransform, tableColumnTransform, -rotationStep));
            }

            if (Input.GetKeyDown(KeyCode.W) && !isRotating)
            {
                SelectNextPiece(+1);
            }

            if (Input.GetKeyDown(KeyCode.S) && !isRotating)
            {
                SelectNextPiece(-1);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
            CheckAllAlignments();

        if (Input.GetMouseButtonDown(1))
        {
            enterPuzzle.EnterPuzzle(false);
            rotatingPiece = null;
            CutVFX();

            if (currentlySelected == null) return;
            currentlySelected.DeselectPiece();
        }

        if (Input.GetKeyDown(KeyCode.Q) && !isRotating) SelectNextColumn(-1);
        if (Input.GetKeyDown(KeyCode.E) && !isRotating) SelectNextColumn(+1);
    }
    private IEnumerator RotatePiece(Transform columnTrans, Transform tableTrans, float angle)
    {
        isRotating = true;

        Quaternion startColumn = columnTrans.rotation;
        Quaternion endColumn = startColumn * Quaternion.Euler(0, angle, 0);

        Quaternion startTable = tableTrans.rotation;
        Quaternion endTable = startTable * Quaternion.Euler(0, angle, 0);

        float elapsed = 0;

        while (elapsed < rotationDuration)
        {
            elapsed += Time.deltaTime;

            float t = elapsed / rotationDuration;

            columnTrans.rotation = Quaternion.Slerp(startColumn, endColumn, t);
            tableTrans.rotation = Quaternion.Slerp(startTable, endTable, t);

            cameraShake.TriggerShake(Time.deltaTime, shakeMagnitude);

            yield return null;
        }

        columnTrans.rotation = endColumn;
        tableTrans.rotation = endTable;

        isRotating = false;
    }

    private void ApplyVFXAlpha(float alpha)
    {
        if (currentlySelected == null) return;
        if (currentlySelected.interiorPieces[piecesCounter] == null) return;

        rotatingPiece = currentlySelected.interiorPieces[piecesCounter];

        foreach (var vfx in rotatingPiece.vfxEffects)
        {
            if (vfx != null)
                vfx.SetFloat("alpha", alpha);
        }
    }

    private void AddFadeOut(InteriorPieceSelector piece)
    {
        if (!fadingPieces.Contains(piece))
            fadingPieces.Add(piece);
    }

    private bool FadeOutPiece(InteriorPieceSelector piece)
    {
        if (piece == null) return true;

        bool allZero = true;

        foreach (var vfx in piece.vfxEffects)
        {
            if (vfx == null) continue;

            float current = vfx.GetFloat("alpha");
            float next = Mathf.MoveTowards(current, 0f, alphaLerpSpeed * Time.deltaTime);
            vfx.SetFloat("alpha", next);

            if (next > 0f)
                allZero = false;
        }

        return allZero;
    }

    private void CutVFX()
    {
        isRotating = false;
        currentAlpha = 0f;
        ApplyVFXAlpha(0f);
    }

    private void SelectNextPiece(int dir)
    {
        AudioManager.Instance.PlaySound("SelectPiece");

        var oldPiece = currentlySelected.interiorPieces[piecesCounter];
        oldPiece.DisableOutline();
        AddFadeOut(oldPiece);

        piecesCounter += dir;
        if (piecesCounter >= currentlySelected.interiorPieces.Length) piecesCounter = 0;
        if (piecesCounter < 0) piecesCounter = currentlySelected.interiorPieces.Length - 1;

        var newPiece = currentlySelected.interiorPieces[piecesCounter];
        newPiece.EnableOutline();
    }

    private void SelectNextColumn(int direction)
    {
        if (allColumns.Count == 0 || hasWon) return;

        if (currentlySelected != null && currentlySelected.interiorPieces.Length > piecesCounter)
        {
            currentlySelected.interiorPieces[piecesCounter].DisableOutline();
            currentlySelected.DeselectPiece();
        }

        currentColumnIndex += direction;
        if (currentColumnIndex < 0) currentColumnIndex = allColumns.Count - 1;
        if (currentColumnIndex >= allColumns.Count) currentColumnIndex = 0;

        var newSelected = allColumns[currentColumnIndex];
        newSelected.SelectedPiece();
    }

    private void CheckAllAlignments()
    {
        foreach (var pair in interiorPieceSelected)
        {
            var column = pair.Key;

            if (column.forward == null ||
                column.lookAtTarget == null ||
                column.columnTransform == null) continue;

            Vector3 pos = column.columnTransform.position;

            Vector3 desiredForward = column.lookAtTarget.position - pos;
            Vector3 actualForward = column.forward.position - pos;

            desiredForward.y = 0;
            actualForward.y = 0;

            column.isAligned = Vector3.Angle(desiredForward, actualForward) < alignThreshold;
        }

        CheckIfPuzzleCompleted();
    }

    private void CheckIfPuzzleCompleted()
    {
        foreach (var pair in interiorPieceSelected)
            if (!pair.Key.isAligned) return;

        foreach (var pair in interiorPieceSelected)
        {
            pair.Key.OnWin();
        }

        OnWinAction?.Invoke(this);
        canRotate = false;
        hasWon = true;
    }
    private void OnDrawGizmos()
    {
        foreach (var pair in interiorPieceSelected)
        {
            var p = pair.Key;

            if (p.forward == null || p.lookAtTarget == null || p.columnTransform == null)
                continue;

            Gizmos.color = Color.green;
            Gizmos.DrawLine(p.columnTransform.position, p.forward.position);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(p.columnTransform.position, p.lookAtTarget.position);
        }
    }

    public void ClearSelection()
    {
        if (currentlySelected != null)
        {
            currentlySelected.DeselectPiece();
            currentlySelected = null;
        }

        rotatingPiece = null;
        currentAlpha = 0f;
    }
}