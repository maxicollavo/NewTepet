using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeightData", menuName = "ScriptableObjects/WeightData", order = 1)]
public class WeightData : ScriptableObject
{
    [System.Serializable]
    public class ObjectWeight
    {
        public ObjectsToPick type;
        public float weight;
    }

    public List<ObjectWeight> objectWeights;

    private Dictionary<ObjectsToPick, float> weightDict;

    public float GetWeight(ObjectsToPick type)
    {
        if (weightDict == null)
        {
            weightDict = new Dictionary<ObjectsToPick, float>();
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