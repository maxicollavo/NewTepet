using System;
using UnityEngine;

public class PlateCollision : MonoBehaviour
{
    [SerializeField] private Plate position;
    [SerializeField] private WeightData weightData; // referencia al ScriptableObject

    public Action<Plate, float> OnCollisionAction;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ObjectType objType))
        {
            var type = objType.type;

            float weight = weightData.GetWeight(type);

            Debug.Log($"Objeto {type} tiene peso {weight}");
            OnCollisionAction?.Invoke(position, weight);
        }
    }
}


public enum Plate
{
    Right,
    Left
}