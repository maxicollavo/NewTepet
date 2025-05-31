using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeightData", menuName = "ScriptableObjects/WeightData", order = 1)]
public class WeightData : ScriptableObject
{
    [System.Serializable]
    public class ObjectWeight
    {
        public ObjectTypeEnum type;
        public float weight;
    }

    public List<ObjectWeight> objectWeights;

    private Dictionary<ObjectTypeEnum, float> weightDict;

    public float GetWeight(ObjectTypeEnum type)
    {
        if (weightDict == null)
        {
            weightDict = new Dictionary<ObjectTypeEnum, float>();
            foreach (var ow in objectWeights)
            {
                weightDict[ow.type] = ow.weight;
            }
        }

        if (weightDict.TryGetValue(type, out var weight))
            return weight;
        else
            return 0f;
    }
}