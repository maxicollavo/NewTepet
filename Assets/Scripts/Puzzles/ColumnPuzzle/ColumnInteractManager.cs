using System.Collections.Generic;
using UnityEngine;

public class ColumnInteractManager : MonoBehaviour
{
    // Diccionario que guarda los ColumnSelected y su estado de "seleccionado"
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
    }
}
