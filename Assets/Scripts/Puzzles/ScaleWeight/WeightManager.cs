using System;
using UnityEngine;

public class WeightManager : MonoBehaviour
{
    public Action<float> OnResultAction;
    [SerializeField] private ObjectCreator creator;

    public float leftWeight = 0f;
    public float rightWeight = 0f;

    public static WeightManager Instance;

    private void Awake()
    {
        Instance = this;

        creator.OnCreateAction += ResultMethod;
    }

    public void ResultMethod(ObjectCreator creator, Plate plate, float weight, bool isAdding = true)
    {
        float value = isAdding ? weight : -weight;

        switch (plate)
        {
            case Plate.Left:
                leftWeight += value;
                break;

            case Plate.Right:
                rightWeight += value;
                break;
        }

        float result = Mathf.Abs(leftWeight - rightWeight);
        Debug.Log(result);
        OnResultAction?.Invoke(result);
    }
}
