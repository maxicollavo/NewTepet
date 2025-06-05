using System;
using System.Collections.Generic;
using UnityEngine;

public class ColumnInteractManager : MonoBehaviour
{
    [HideInInspector] public Dictionary<ColumnSelected, bool> columnSelecteds = new Dictionary<ColumnSelected, bool>();
    public Action<ColumnInteractManager> OnWinAction;

    [SerializeField] float rotationSpeed = 50f;
    [SerializeField] float alignThreshold = 10f;
    [SerializeField] float winThreshold = 0.1f;
    [SerializeField] EnterColumnPuzzle enterPuzzle;

    private ColumnSelected currentlySelected;
    private Transform forward;
    private Transform lookAtTarget;

    private bool isAligning = false;
    private Quaternion targetRotation;

    public bool canRotate;

    public void OnSelectedMethod(bool isSelected, ColumnSelected selected, Transform greenPoint, Transform bluePoint)
    {
        if (currentlySelected != null && currentlySelected != selected)
            currentlySelected.DeselectPiece();

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

        if (isAligning && currentlySelected != null)
        {
            Transform columnTransform = currentlySelected.columnToRotate.transform;

            columnTransform.rotation = Quaternion.RotateTowards(
                columnTransform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            float angle = Quaternion.Angle(columnTransform.rotation, targetRotation);

            if (angle < 0.1f)
            {
                isAligning = false;

                Vector3 diff = lookAtTarget.position - forward.position;
                diff.y = 0;

                if (diff.magnitude < winThreshold)
                {
                    Debug.Log("¡Ganó!");
                    columnTransform.rotation = targetRotation;
                    Win();
                }
            }
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
        if (forward == null || lookAtTarget == null || currentlySelected == null) return;

        var desiredForward = lookAtTarget.position - currentlySelected.transform.position;
        var actualForward = forward.position - currentlySelected.transform.position;
        var angle = Vector3.Angle(desiredForward, actualForward);


        if(angle < alignThreshold)
        {
            isAligning = true;

            Transform columnTransform = currentlySelected.columnToRotate.transform;
            Vector3 columnPos = columnTransform.position;

            Vector3 currentDir = (forward.position - columnPos).normalized;
            Vector3 targetDir = (lookAtTarget.position - columnPos).normalized;

            currentDir.y = 0;
            targetDir.y = 0;

            Quaternion deltaRotation = Quaternion.FromToRotation(currentDir, targetDir);
            targetRotation = deltaRotation * columnTransform.rotation;
        }

        //Vector3 diff = lookAtTarget.position - forward.position;
        //diff.y = 0;
        //float distance = diff.magnitude;

        if (angle < winThreshold)
        {
            Win();
        }
        //else if (distance < alignThreshold)
        //{
        //    isAligning = true;

        //    Transform columnTransform = currentlySelected.columnToRotate.transform;
        //    Vector3 columnPos = columnTransform.position;

        //    Vector3 currentDir = (forward.position - columnPos).normalized;
        //    Vector3 targetDir = (lookAtTarget.position - columnPos).normalized;

        //    currentDir.y = 0;
        //    targetDir.y = 0;

        //    Quaternion deltaRotation = Quaternion.FromToRotation(currentDir, targetDir);
        //    targetRotation = deltaRotation * columnTransform.rotation;
        //}
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
