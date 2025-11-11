using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class ColumnInteractManager : MonoBehaviour
{
    [HideInInspector] public Dictionary<InteriorPieceSelector, bool> interiorPieceSelected = new Dictionary<InteriorPieceSelector, bool>();
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
    [HideInInspector] public bool hasWon;

    [Header("Interior Pieces Settings")]
    private int piecesCounter;
    private int oldPieceCounter;

    [Header("Columnas")]
    [SerializeField] private List<ColumnSelected> allColumns = new List<ColumnSelected>();
    private int currentColumnIndex = 0;
    private bool isRotating;

    [Header("Camera Shake")]
    [SerializeField] private CameraShake cameraShake;
    [SerializeField] private float shakeMagnitude;

    public void OnSelectedMethod(bool isSelected, ColumnSelected selected)
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

            oldPieceCounter = piecesCounter;
            piecesCounter = oldPieceCounter;
            forward = currentlySelected.interiorPieces[piecesCounter].forward;
            lookAtTarget = currentlySelected.interiorPieces[piecesCounter].lookAtTarget;

            if (currentlySelected.interiorPieces.Length > 0)
            {
                currentlySelected.interiorPieces[piecesCounter].EnableOutline();
            }
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

        if (currentlySelected != null && !currentlySelected.interiorPieces[piecesCounter].hasWon)
        {
            Transform columnTransform = currentlySelected.interiorPieces[piecesCounter].columnTransform;
            Transform tableColumnTransform = currentlySelected.interiorPieces[piecesCounter].transform.parent.transform;
            VisualEffect[] vfxEffects = currentlySelected.interiorPieces[piecesCounter].vfxEffects;

            if (Input.GetKey(KeyCode.A))
            {
                columnTransform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
                tableColumnTransform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
                cameraShake.TriggerShake(Time.deltaTime, shakeMagnitude);
                isRotating = true;

                Debug.Log($"Los efectos visuales de la columna {columnTransform.gameObject} son {vfxEffects[0].gameObject} y {vfxEffects[1].gameObject}");
            }
            else if (Input.GetKey(KeyCode.D))
            {
                columnTransform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
                tableColumnTransform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
                cameraShake.TriggerShake(Time.deltaTime, shakeMagnitude);
                isRotating = true;
            }

            foreach (var vfx in vfxEffects)
            {
                if (vfx == null) continue;

                if (isRotating)
                {
                    foreach (var vfxEffect in vfxEffects)
                        vfx.SetFloat("alpha", 1);
                    Debug.Log("entro a play");
                }
                else
                    foreach (var vfxEffect in vfxEffects)
                        vfx.SetFloat("alpha", 0);
            }

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
            Debug.Log("Llama a Deselect Piece");
            currentlySelected.DeselectPiece();
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            SelectNextColumn(-1);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            SelectNextColumn(1);
        }
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
            InteriorPieceSelector column = pair.Key;

            if (column.forward == null || column.lookAtTarget == null || column.columnTransform == null)
            {
                continue;
            }

            Transform columnTransform = column.columnTransform;
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
        foreach (var pair in interiorPieceSelected)
        {
            if (!pair.Key.isAligned)
            {
                return;
            }
        }

        foreach (var pair in interiorPieceSelected)
        {
            AlignColumn(pair.Key);
            pair.Key.OnWin();
        }

        OnWinAction?.Invoke(this);
        canRotate = false;
        hasWon = true;
    }

    private void AlignColumn(InteriorPieceSelector column)
    {
        Transform columnTransform = column.columnTransform;
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

    private void OnDrawGizmos()
    {
        foreach (var pair in interiorPieceSelected)
        {
            var piece = pair.Key;
            if (piece.forward != null && piece.lookAtTarget != null && piece.columnTransform != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(piece.columnTransform.position, piece.forward.position);

                Gizmos.color = Color.red;
                Gizmos.DrawLine(piece.columnTransform.position, piece.lookAtTarget.position);
            }
        }
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
