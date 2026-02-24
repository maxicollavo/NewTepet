using System;
using UnityEngine;

public class WeightManager : MonoBehaviour
{
    public Action<bool, float, float> OnResultAction;
    [SerializeField] private ObjectCreator creator;

    public float leftWeight = 0f;
    public float rightWeight = 0f;

    public static WeightManager Instance;

    private void Awake()
    {
        Instance = this;

        creator.OnCreateAction += ResultMethod;
    }

    public void ResultMethod(Plate plate, float l, float r, bool canOpenDoor, bool isAdding = true)
    {
        leftWeight = l;
        rightWeight = r;

        OnResultAction.Invoke(canOpenDoor, leftWeight, rightWeight);
    }
}
