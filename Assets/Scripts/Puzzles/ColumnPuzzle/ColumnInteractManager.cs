using System.Collections.Generic;
using UnityEngine;

public class ColumnInteractManager : MonoBehaviour
{
    [HideInInspector] public Dictionary<ColumnSelected, bool> columnSelecteds = new Dictionary<ColumnSelected, bool>();

    [SerializeField] float rotationSpeed;
    [SerializeField] EnterColumnPuzzle enterPuzzle;

    private void Awake()
    {
        foreach (var column in columnSelecteds)
        {
            column.Key.OnSelectedAction += OnSelectedMethod;
        }
    }

    private void OnSelectedMethod(bool isSelected, ColumnSelected selected)
    {
        var keys = new List<ColumnSelected>(columnSelecteds.Keys);

        foreach (var column in keys)
        {
            if (column != selected && columnSelecteds[column])
            {
                column.DeselectPiece();
                columnSelecteds[column] = false;
            }
        }

        if (columnSelecteds.ContainsKey(selected))
        {
            columnSelecteds[selected] = isSelected;
        }
    }


    private void Update()
    {
        foreach (var kvp in columnSelecteds)
        {
            if (kvp.Value)
            {
                var selectedTransform = kvp.Key.transform;

                if (Input.GetKey(KeyCode.A))
                {
                    selectedTransform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime);
                }
                if (Input.GetKey(KeyCode.D))
                {
                    selectedTransform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
                }
                break;
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            enterPuzzle.EnterPuzzle(false);
        }
    }
}
