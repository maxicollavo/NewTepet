using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCreator : MonoBehaviour
{
    [SerializeField] private List<ObjectPrefabPair> prefabsByType;
    public Action<ObjectCreator, Plate, float, bool> OnCreateAction;

    [SerializeField] private WeightData weightData;

    public static ObjectCreator Instance;

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

    public void InstantiateObject(ObjectsToPick type, Vector3 position, Plate sidePlate)
    {
        if (!prefabDict.TryGetValue(type, out var prefab))
        {
            Debug.LogWarning($"No se encontró prefab para tipo {type}");
            return;
        }

        GameObject obj = Instantiate(prefab, position, Quaternion.identity);
        Rigidbody rb = obj.GetComponent<Rigidbody>();

        if (obj.TryGetComponent(out ObjectType objectType))
        {
            objectType.type = type;
            objectType.weight = weightData.GetWeight(type);

            StartCoroutine(ChangeMass(rb));
        }

        if (obj.TryGetComponent(out PickToInventory pick))
        {
            pick.isOnScale = true;
            pick.plateSide = sidePlate;
        }

        Debug.Log($"Instanciado: {type} con peso {objectType.weight} en el plato {sidePlate}");
        OnCreateAction?.Invoke(this, sidePlate, objectType.weight, true);

    }

    public IEnumerator ChangeMass(Rigidbody rb)
    {
        yield return new WaitForSeconds(2f);
        rb.mass = 0.1f;
    }
}

[System.Serializable]
public class ObjectPrefabPair
{
    public ObjectsToPick type;
    public GameObject prefab;
}
