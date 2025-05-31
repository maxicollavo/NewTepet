using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCreator : MonoBehaviour
{
    [SerializeField] private GameObject prefabToInstantiate;
    [SerializeField] private WeightData weightData;

    public static ObjectCreator Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void InstantiateObject(ObjectTypeEnum type, Vector3 position)
    {
        GameObject obj = Instantiate(prefabToInstantiate, position, Quaternion.identity);

        if (obj.TryGetComponent(out ObjectType objectType))
        {
            objectType.type = type;

            objectType.weight = weightData.GetWeight(type);

            Debug.Log($"Instanciado: {type} con peso {objectType.weight}");
        }
    }
}
