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

        if (sidePlate == Plate.Left)
            leftPlateObjects.Add(obj);
        else if (sidePlate == Plate.Right)
            rightPlateObjects.Add(obj);

        if (obj.TryGetComponent(out ObjectType objectType))
        {
            objectType.type = type;
            objectType.weight = weightData.GetWeight(type);

            Plate otherPlate = sidePlate == Plate.Left ? Plate.Right : Plate.Left;
            float otherWeight = otherPlate == Plate.Left ? WeightManager.Instance.leftWeight : WeightManager.Instance.rightWeight;

            List<GameObject> otherPlateObjects = otherPlate == Plate.Left ? leftPlateObjects : rightPlateObjects;

            if (type == ObjectsToPick.Heart &&
                Mathf.Approximately(otherWeight, 1f) &&
                otherPlateObjects.Count == 1 &&
                otherPlateObjects[0].GetComponent<ObjectType>().type == ObjectsToPick.Feather)
            {
                Debug.Log("Corazón compensado: pasa de 51 a 1");
                objectType.weight = 1f;
                shouldOpenDoor = true;
            }
            else if (type == ObjectsToPick.Feather &&
                     Mathf.Approximately(otherWeight, 51f) &&
                     otherPlateObjects.Count == 1 &&
                     otherPlateObjects[0].GetComponent<ObjectType>().type == ObjectsToPick.Heart)
            {
                Debug.Log("Pluma compensada: pasa de 1 a 51");
                objectType.weight = 51f;
                shouldOpenDoor = true;
            }

            StartCoroutine(ChangeMass(rb, objectType.weight));
        }

        if (obj.TryGetComponent(out PickToInventory pick))
        {
            pick.isOnScale = true;
            pick.plateSide = sidePlate;
        }

        Debug.Log($"Instanciado: {type} con peso final {objectType.weight} en el plato {sidePlate}");
        OnCreateAction?.Invoke(this, sidePlate, objectType.weight, shouldOpenDoor, true);
    }

    public IEnumerator ChangeMass(Rigidbody rb, float weight)
    {
        yield return new WaitForSeconds(2f);
        rb.mass = weight > 20f ? weight / 2f : weight;
    }

    public void RemoveSpawnedObject(GameObject obj)
    {
        if (obj.TryGetComponent(out PickToInventory pick))
        {
            if (pick.plateSide == Plate.Left)
                leftPlateObjects.Remove(obj);
            else if (pick.plateSide == Plate.Right)
                rightPlateObjects.Remove(obj);
        }
    }

}

[System.Serializable]
public class ObjectPrefabPair
{
    public ObjectsToPick type;
    public GameObject prefab;
}
