using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectType : MonoBehaviour
{
    [HideInInspector]
    public float weight;
    [HideInInspector]
    public ObjectTypeEnum type;
}

public enum ObjectTypeEnum
{
    Feather,
    Stone,
    Knife,
    Canopo,
    Djed,
    Heart
}
