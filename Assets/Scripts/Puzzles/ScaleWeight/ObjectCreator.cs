using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCreator : MonoBehaviour
{
    [SerializeField] private List<ObjectPrefabPair> prefabsByType;
    private List<GameObject> leftPlateObjects = new List<GameObject>();
    private List<GameObject> rightPlateObjects = new List<GameObject>();

    public Action<ObjectCreator, Plate, float, bool, bool> OnCreateAction;

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
        Debug.Log($"Se instanció el siguiente objeto: {obj}");
        Rigidbody rb = obj.GetComponent<Rigidbody>();

        if (sidePlate == Plate.Left)
            leftPlateObjects.Add(obj);
        else if (sidePlate == Plate.Right)
            rightPlateObjects.Add(obj);
        Debug.Log($"El objeto instanciado lo hizo en el plato {sidePlate}");

        if (obj.TryGetComponent(out ObjectType objectType))
        {
            objectType.type = type;
            Debug.Log($"Se le asignó como tipo de objeto el siguiente: {type}");

            objectType.weight = weightData.GetWeight(type);
            Debug.Log($"Se le asignó como peso de objeto el siguiente: {weightData.GetWeight(type)}");

            Plate otherPlate = sidePlate == Plate.Left ? Plate.Right : Plate.Left;
            Debug.Log($"El otro plato es el siguiente: {otherPlate}");
            float otherWeight = otherPlate == Plate.Left ? WeightManager.Instance.leftWeight : WeightManager.Instance.rightWeight;
            Debug.Log($"El peso del otro plato es el siguiente: {otherWeight}");

            List<GameObject> otherPlateObjects = otherPlate == Plate.Left ? leftPlateObjects : rightPlateObjects;
            Debug.Log($"Los objetos que están en el otro plato son los siguientes {otherPlateObjects}");
            List<GameObject> myPlateObjects = sidePlate == Plate.Left ? leftPlateObjects : rightPlateObjects;
            Debug.Log($"Los objetos que están en este plato son los siguientes {myPlateObjects}");

            if (type == ObjectsToPick.Heart &&
                otherPlateObjects.Count == 1 && myPlateObjects.Count == 1 &&
                otherPlateObjects[0].GetComponent<ObjectType>().type == ObjectsToPick.Feather)
            {
                objectType.weight = 1f;
                shouldOpenDoor = true;
                Debug.Log($"{objectType.type} compensado: pasa de 51 a 1, ");
            }
            else if (type == ObjectsToPick.Feather &&
                     otherPlateObjects.Count == 1 && myPlateObjects.Count == 1 &&
                     otherPlateObjects[0].GetComponent<ObjectType>().type == ObjectsToPick.Heart)
            {
                objectType.weight = 51f;
                shouldOpenDoor = true;
                Debug.Log($"{objectType.type} compensado: pasa de 1 a 51, ");
            }

            StartCoroutine(ChangeMass(rb));
        }

        if (obj.TryGetComponent(out PickToInventory pick))
        {
            pick.isOnScale = true;
            pick.plateSide = sidePlate;
        }

        Debug.Log($"Instanciado: {type} con peso final {objectType.weight} en el plato {sidePlate}");
        OnCreateAction?.Invoke(this, sidePlate, objectType.weight, shouldOpenDoor, true);
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
                    WeightManager.Instance.UpdateWeight(true, leftType.weight, shouldOpenDoor);
                    WeightManager.Instance.UpdateWeight(false, rightType.weight, shouldOpenDoor);
                }
            }
        }
    }

}

[System.Serializable]
public class ObjectPrefabPair
{
    public ObjectsToPick type;
    public GameObject prefab;
}
