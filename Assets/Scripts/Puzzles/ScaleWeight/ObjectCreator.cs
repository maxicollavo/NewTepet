using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectCreator : MonoBehaviour
{
    [SerializeField] private List<ObjectPrefabPair> prefabsByType;
    private List<GameObject> leftPlateObjects = new List<GameObject>();
    private List<GameObject> rightPlateObjects = new List<GameObject>();

    public Action<Plate, float, float, bool, bool> OnCreateAction;

    [SerializeField] private WeightData weightData;

    [SerializeField] private List<BoxCollider> platesColl;

    public static ObjectCreator Instance;

    bool shouldOpenDoor;
    public bool canPick;

    private Dictionary<ObjectsToPick, GameObject> prefabDict;

    private void Awake()
    {
        Instance = this;

        prefabDict = new Dictionary<ObjectsToPick, GameObject>();
        foreach (var pair in prefabsByType)
        {
            if (!prefabDict.ContainsKey(pair.type))
                prefabDict.Add(pair.type, pair.prefab);
        }
    }

    private void Start()
    {
        canPick = true;
    }

    public void InstantiateObject(ObjectsToPick type, Vector3 position, Plate sidePlate)
    {
        if (!prefabDict.TryGetValue(type, out var prefab))
        {
            Debug.LogWarning($"No se encontró prefab para tipo {type}");
            return;
        }

        GameObject obj = Instantiate(prefab, position, Quaternion.identity);
        Rigidbody rb = obj.GetComponent<Rigidbody>();

        switch (sidePlate)
        {
            case Plate.Right:
                rightPlateObjects.Add(obj);
                break;
            case Plate.Left:
                leftPlateObjects.Add(obj);
                break;
            case Plate.None:
                StartCoroutine(ChangeMass(rb));
                return;
        }

        if (obj.TryGetComponent(out ObjectType objectType))
        {
            objectType.type = type;

            objectType.weight = weightData.GetWeight(type);

            Plate otherPlate = sidePlate == Plate.Left ? Plate.Right : Plate.Left;
            float otherWeight = otherPlate == Plate.Left ? WeightManager.Instance.leftWeight : WeightManager.Instance.rightWeight;

            List<GameObject> otherPlateObjects = otherPlate == Plate.Left ? leftPlateObjects : rightPlateObjects;
            List<GameObject> myPlateObjects = sidePlate == Plate.Left ? leftPlateObjects : rightPlateObjects;

            if (type == ObjectsToPick.Heart &&
                otherPlateObjects.Count == 1 && myPlateObjects.Count == 1 &&
                otherPlateObjects[0].GetComponent<ObjectType>().type == ObjectsToPick.Feather)
            {
                objectType.weight = 1f;
                shouldOpenDoor = true;
            }
            else if (type == ObjectsToPick.Feather &&
                     otherPlateObjects.Count == 1 && myPlateObjects.Count == 1 &&
                     otherPlateObjects[0].GetComponent<ObjectType>().type == ObjectsToPick.Heart)
            {
                objectType.weight = 51f;
                shouldOpenDoor = true;
            }

            StartCoroutine(ChangeMass(rb));
        }

        if (obj.TryGetComponent(out PickToInventory pick))
        {
            pick.isOnScale = true;
            pick.plateSide = sidePlate;
        }

        UpdateScaleWeights();
    }

    public IEnumerator ChangeMass(Rigidbody rb)
    {
        yield return new WaitForSeconds(2f);
        rb.mass = 0.1f;
    }

    public void RemoveSpawnedObject(GameObject obj)
    {
        if (obj.TryGetComponent(out PickToInventory pick))
        {
            if (pick.plateSide == Plate.Left)
                leftPlateObjects.Remove(obj);
            else if (pick.plateSide == Plate.Right)
                rightPlateObjects.Remove(obj);

            if (leftPlateObjects.Count == 1 && rightPlateObjects.Count == 1)
            {
                var leftType = leftPlateObjects[0].GetComponent<ObjectType>();
                var rightType = rightPlateObjects[0].GetComponent<ObjectType>();

                if (leftType == null || rightType == null) return;

                if ((leftType.type == ObjectsToPick.Feather && rightType.type == ObjectsToPick.Heart) || (leftType.type == ObjectsToPick.Heart && rightType.type == ObjectsToPick.Feather))
                {
                    leftType.weight = 1;
                    rightType.weight = 1;
                    shouldOpenDoor = true;
                }
            }
        }

        UpdateScaleWeights();
    }

    public void UpdateScaleWeights()
    {
        OnCreateAction?.Invoke(Plate.None, leftPlateObjects.Sum(o => o.GetComponent<ObjectType>().weight), rightPlateObjects.Sum(o => o.GetComponent<ObjectType>().weight), shouldOpenDoor, true);
    }
}


[System.Serializable]
public class ObjectPrefabPair
{
    public ObjectsToPick type;
    public GameObject prefab;
}
