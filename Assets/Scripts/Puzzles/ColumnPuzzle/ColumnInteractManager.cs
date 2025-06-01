using System.Collections.Generic;
using UnityEngine;

public class ColumnInteractManager : MonoBehaviour
{
    [HideInInspector] public Dictionary<ColumnSelected, bool> columnSelecteds = new Dictionary<ColumnSelected, bool>();

    [SerializeField] float rotationSpeed;
    [SerializeField] EnterColumnPuzzle enterPuzzle;

    private ColumnSelected currentlySelected;
    public bool canRotate;

    public void OnSelectedMethod(bool isSelected, ColumnSelected selected)
    {
        Debug.Log($"OnSelectedMethod llamado por: {selected.name} con isSelected: {isSelected}");

        if (currentlySelected != null && currentlySelected != selected)
        {
            Debug.Log($"Deseleccionando {currentlySelected.name}");
            currentlySelected.DeselectPiece();
        }

        if (isSelected)
        {
            Debug.Log($"Seleccionando {selected.name}");
            currentlySelected = selected;
        }
        else
        {
            Debug.Log($"Desmarcando {selected.name}");
            currentlySelected = null;
        }
    }


    private void Update()
    {
        if (!canRotate) return;

        if (currentlySelected != null)
        {
            var selectedTransform = currentlySelected.columnToRotate.transform;

            if (Input.GetKey(KeyCode.A))
            {
                selectedTransform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
            }
            if (Input.GetKey(KeyCode.D))
            {
                selectedTransform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            enterPuzzle.EnterPuzzle(false);

            if (currentlySelected == null) return;
            currentlySelected.DeselectPiece();
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
