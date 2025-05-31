using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectType : MonoBehaviour
{
    public float weight;
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
