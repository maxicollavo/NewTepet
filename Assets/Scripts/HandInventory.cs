using System.Collections.Generic;
using UnityEngine;

public class HandInventory : MonoBehaviour
{
    [SerializeField] private List<HandObjectEntry> handObjectList;
    private Dictionary<ObjectsToPick, GameObject> handObj = new();

    public static bool hasObjInHand;

    private void Awake()
    {
        foreach (var entry in handObjectList)
        {
            if (!handObj.ContainsKey(entry.key))
            {
                handObj.Add(entry.key, entry.value);
            }
        }
    }

    public void ShowObjectInHand(ObjectsToPick obj)
    {
        if (handObj.TryGetValue(obj, out GameObject go))
        {
            go.SetActive(true);
        }
        else
        {
            Debug.LogWarning($"No se encontró el objeto en mano para: {obj}");
        }
    }

    public static bool IsHoldingSomething()
    {
        return hasObjInHand;
    }
}