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

    private bool isAligning = false;
    private Quaternion targetRotation;

    public bool canRotate;

    public void OnSelectedMethod(bool isSelected, ColumnSelected selected, Transform column, Transform greenPoint, Transform bluePoint)
    {
        if (currentlySelected != null && currentlySelected != selected)
            currentlySelected.DeselectPiece();

        if (column != null)
            columnTransform = column;

        if (isSelected)
        {
            currentlySelected = selected;
            forward = greenPoint;
            lookAtTarget = bluePoint;
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
            Transform columnTransform = currentlySelected.columnToRotate.transform;

            if (Input.GetKey(KeyCode.A))
                columnTransform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);

            if (Input.GetKey(KeyCode.D))
                columnTransform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

            if (Input.GetKeyDown(KeyCode.Space))
                CheckAlignment();
        }

        if (Input.GetMouseButtonDown(1))
        {
            enterPuzzle.EnterPuzzle(false);
            if (currentlySelected == null) return;
            currentlySelected.DeselectPiece();
        }
    }

    private void CheckAlignment()
    {
        if (forward == null || lookAtTarget == null || currentlySelected == null || columnTransform == null) return;

        Vector3 pos = columnTransform.position;

        var desiredForward = lookAtTarget.position - pos;
        var actualForward = forward.position - pos;

        desiredForward.y = 0;
        actualForward.y = 0;

        var angle = Vector3.Angle(desiredForward, actualForward);
        Debug.Log($"El angulo de la columna {columnTransform.gameObject} es {angle}");

        if (angle < alignThreshold)
        {
            currentlySelected.isAligned = true;
        }
        else
        {
            currentlySelected.isAligned = false;
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

        //Sonido de alineamiento (puede ser tipo bloqueo)
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
