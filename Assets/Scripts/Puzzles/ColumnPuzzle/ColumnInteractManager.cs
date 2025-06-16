using System;
using System.Collections.Generic;
using UnityEngine;

public class ColumnInteractManager : MonoBehaviour
{
    [HideInInspector] public Dictionary<ColumnSelected, bool> columnSelecteds = new Dictionary<ColumnSelected, bool>();
    public Action<ColumnInteractManager> OnWinAction;

    [SerializeField] float rotationSpeed = 50f;
    [SerializeField] float alignThreshold;
    [SerializeField] float winThreshold;
    [SerializeField] EnterColumnPuzzle enterPuzzle;

    private ColumnSelected currentlySelected;
    private Transform columnTransform;
    private Transform forward;
    private Transform lookAtTarget;

    private Quaternion targetRotation;

    public bool canRotate;

    [Header("Interior Pieces Settings")]
    private int piecesCounter;
    private int oldPieceCounter;

    public void OnSelectedMethod(bool isSelected, ColumnSelected selected, Transform column, Transform columnForward, Transform target)
    {
        if (currentlySelected != null && currentlySelected != selected)
        {
            oldPieceCounter = piecesCounter;

            if (currentlySelected.interiorPieces.Length > piecesCounter)
            {
                currentlySelected.interiorPieces[piecesCounter].DisableOutline();
            }

            currentlySelected.DeselectPiece();
        }

        if (isSelected)
        {
            currentlySelected = selected;
            forward = columnForward;
            lookAtTarget = target;

            oldPieceCounter = piecesCounter;
            piecesCounter = oldPieceCounter;

            if (currentlySelected.interiorPieces.Length > 0)
            {
                currentlySelected.interiorPieces[piecesCounter].EnableOutline();
            }
        }

        if (column != null)
            columnTransform = column;

        if (isSelected)
        {
            currentlySelected = selected;
            forward = columnForward;
            lookAtTarget = target;
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

        if (currentlySelected != null && !currentlySelected.hasWon)
        {
            Transform columnTransform = currentlySelected.interiorPieces[piecesCounter].columnTransform;

            if (Input.GetKey(KeyCode.A))
                columnTransform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

            if (Input.GetKey(KeyCode.D))
                columnTransform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);

            if (Input.GetKeyDown(KeyCode.W))
            {
                AudioManager.Instance.PlaySound("SelectPiece");

                var currentlyPieceSelected = currentlySelected.interiorPieces[piecesCounter];
                currentlyPieceSelected.DisableOutline();

                piecesCounter++;
                if (piecesCounter > currentlySelected.interiorPieces.Length - 1)
                    piecesCounter = 0;

                currentlyPieceSelected = currentlySelected.interiorPieces[piecesCounter];
                currentlyPieceSelected.EnableOutline();
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                AudioManager.Instance.PlaySound("SelectPiece");

                var currentlyPieceSelected = currentlySelected.interiorPieces[piecesCounter];
                currentlyPieceSelected.DisableOutline();

                piecesCounter--;
                if (piecesCounter < 0)
                    piecesCounter = currentlySelected.interiorPieces.Length - 1;

                currentlyPieceSelected = currentlySelected.interiorPieces[piecesCounter];
                currentlyPieceSelected.EnableOutline();
            }

        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CheckAllAlignments();
        }

        if (Input.GetMouseButtonDown(1))
        {
            enterPuzzle.EnterPuzzle(false);
            if (currentlySelected == null) return;
            currentlySelected.DeselectPiece();
        }
    }

    private void CheckAllAlignments()
    {
        foreach (var pair in columnSelecteds)
        {
            ColumnSelected column = pair.Key;

            if (column.forward == null || column.lookAtTarget == null || column.columnToRotate == null) continue;

            Transform columnTransform = column.columnToRotate.transform;
            Vector3 pos = columnTransform.position;

            var desiredForward = column.lookAtTarget.position - pos;
            var actualForward = column.forward.position - pos;

            desiredForward.y = 0;
            actualForward.y = 0;

            var angle = Vector3.Angle(desiredForward, actualForward);

            column.isAligned = angle < alignThreshold;
        }

        CheckIfPuzzleCompleted();
    }

    private void CheckIfPuzzleCompleted()
    {
        foreach (var pair in columnSelecteds)
        {
            if (!pair.Key.isAligned) return;
        }

        foreach (var pair in columnSelecteds)
        {
            AlignColumn(pair.Key);
            pair.Key.OnWin();
        }

        OnWinAction?.Invoke(this);
        canRotate = false;
    }

    private void AlignColumn(ColumnSelected column)
    {
        Transform columnTransform = column.columnToRotate.transform;
        Vector3 pos = columnTransform.position;
        Transform forwardPoint = column.forward;
        Transform lookAtPoint = column.lookAtTarget;

        if (forwardPoint == null || lookAtPoint == null)
        {
            return;
        }

        Vector3 currentDir = (forwardPoint.position - pos).normalized;
        Vector3 targetDir = (lookAtPoint.position - pos).normalized;

        currentDir.y = 0;
        targetDir.y = 0;

        Quaternion deltaRotation = Quaternion.FromToRotation(currentDir, targetDir);
        Quaternion targetRotation = deltaRotation * columnTransform.rotation;

        columnTransform.rotation = targetRotation;

        //Sonido de alineamiento con algún tipo de eco (puede ser tipo bloqueo)
    }

    public void ClearSelection()
    {
        if (currentlySelected != null)
        {
            currentlySelected.DeselectPiece();
            currentlySelected = null;
        }
    }
}
