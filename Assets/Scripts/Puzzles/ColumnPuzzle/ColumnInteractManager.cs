using System;
using System.Collections.Generic;
using UnityEngine;

public class ColumnInteractManager : MonoBehaviour
{
    [HideInInspector] public Dictionary<ColumnSelected, bool> columnSelecteds = new Dictionary<ColumnSelected, bool>();
    public Action<ColumnInteractManager> OnWinAction;
    [SerializeField] float rotationSpeed;
    [SerializeField] EnterColumnPuzzle enterPuzzle;

    private ColumnSelected currentlySelected;
    public bool canRotate;

    public void OnSelectedMethod(bool isSelected, ColumnSelected selected)
    {
        if (currentlySelected != null && currentlySelected != selected)
        {
            currentlySelected.DeselectPiece();
        }

        if (isSelected)
        {
            currentlySelected = selected;
        }
        else
        {
            currentlySelected = null;
        }
    }

    private void Update()
    {
        if (!canRotate) return;

        if (currentlySelected != null)
        {
            var selectedTransform = currentlySelected.columnToRotate.transform;

            if (currentlySelected.hasWon) return;

            if (Input.GetKey(KeyCode.A))
            {
                selectedTransform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.D))
            {
                selectedTransform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            }

            float yRotation = selectedTransform.eulerAngles.y;
            if (yRotation > 180f) yRotation -= 360f;

            if (currentlySelected.isLeft && Mathf.Abs(yRotation - (-17f)) < 0.5f)
            {
                currentlySelected.OnWin();
                CheckIfPuzzleCompleted();
            }
            else if (!currentlySelected.isLeft && Mathf.Abs(yRotation - (-22f)) < 0.5f)
            {
                currentlySelected.OnWin();
                CheckIfPuzzleCompleted();
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            enterPuzzle.EnterPuzzle(false);

            if (currentlySelected == null) return;
            currentlySelected.DeselectPiece();
        }
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
