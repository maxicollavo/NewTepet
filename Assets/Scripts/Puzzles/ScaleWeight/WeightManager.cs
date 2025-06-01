using System;
using UnityEngine;

public class WeightManager : MonoBehaviour
{
    public Action<float> OnResultAction;
    [SerializeField] private PlateCollision[] collisions;

    public float leftWeight = 0f;
    public float rightWeight = 0f;

    public static WeightManager Instance;

    private void Awake()
    {
        Instance = this;

        foreach (var col in collisions)
        {
            col.OnCollisionAction += OnCollisionMethod;
        }
    }

    private void OnCollisionMethod(Plate plate, float weight)
    {
        switch (plate)
        {
            case Plate.Left:
                leftWeight += weight;
                break;

            case Plate.Right:
                rightWeight += weight;
                break;
        }

        float result = leftWeight > rightWeight ? leftWeight - rightWeight : rightWeight - leftWeight;
        Debug.Log(result);
        OnResultAction?.Invoke(result);
    }
}
