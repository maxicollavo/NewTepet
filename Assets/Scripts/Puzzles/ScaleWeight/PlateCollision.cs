using System;
using UnityEngine;

public class PlateCollision : MonoBehaviour
{
    [SerializeField] private Plate position;
    [SerializeField] private WeightData weightData;

    public Action<Plate, float> OnCollisionAction;

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ObjectType objType))
        {
            var type = objType.type;

            float weight = weightData.GetWeight(type);

            OnCollisionAction?.Invoke(position, weight);
        }
    }
}

public enum Plate
{
    Right,
    Left,
    None
}