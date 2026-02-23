using System;
using UnityEngine;

public class WeightManager : MonoBehaviour
{
    public Action<float, bool> OnResultAction;
    [SerializeField] private ObjectCreator creator;

    public float leftWeight = 0f;
    public float rightWeight = 0f;

    public static WeightManager Instance;

    private void Awake()
    {
        Instance = this;

        creator.OnCreateAction += ResultMethod;
    }

    public void ResultMethod(ObjectCreator creator, Plate plate, float weight, bool canOpenDoor, bool isAdding = true)
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
        OnResultAction?.Invoke(result, canOpenDoor);
    }

    public void UpdateWeight(bool isLeftWeigth, float weight, bool canOpenDoor)
    {
        if (isLeftWeigth)
            leftWeight = weight;
        else
            rightWeight = weight;
        float result = Mathf.Abs(leftWeight - rightWeight);
        OnResultAction?.Invoke(result, canOpenDoor);


    }
}
