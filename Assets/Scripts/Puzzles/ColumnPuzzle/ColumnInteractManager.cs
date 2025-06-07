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
        Debug.Log("Ángulo actual: " + angle);

        Debug.DrawLine(pos, pos + desiredForward.normalized * 2, Color.green, 2f);

        Debug.DrawLine(pos, pos + actualForward.normalized * 2, Color.red, 2f);

        Debug.DrawLine(pos, lookAtTarget.position, Color.blue, 2f);

        if (angle < alignThreshold)
        {
            Transform columnTransform = currentlySelected.columnToRotate.transform;
            Vector3 columnPos = columnTransform.position;

            Vector3 currentDir = (forward.position - columnPos).normalized;
            Vector3 targetDir = (lookAtTarget.position - columnPos).normalized;

            currentDir.y = 0;
            targetDir.y = 0;

            Quaternion deltaRotation = Quaternion.FromToRotation(currentDir, targetDir);
            targetRotation = deltaRotation * columnTransform.rotation;

            columnTransform.rotation = targetRotation;

            Win();
        }
        else if (angle < winThreshold)
        {
            Win();
        }

    }

    private void Win()
    {
        currentlySelected.OnWin();
        CheckIfPuzzleCompleted();
    }

    private void CheckIfPuzzleCompleted()
    {
        foreach (var pair in columnSelecteds)
        {
            if (!pair.Key.hasWon) return;
        }

        OnWinAction?.Invoke(this);
        canRotate = false;
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
