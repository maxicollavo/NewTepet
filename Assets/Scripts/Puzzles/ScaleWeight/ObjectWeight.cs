using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectWeight : MonoBehaviour
{
    public enum ObjectType
    {
        One,
        Two,
        Three
    }

    private Dictionary<ObjectType, float> objectMasses = new Dictionary<ObjectType, float>()
    {
        { ObjectType.One, 1f},
        { ObjectType.Two, 2f},
        { ObjectType.Three, 3f},
    };

    public ObjectType selectedObject;
    private ObjectType _lastType;

    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        UpdateMass();
    }

    private void Update()
    {
        if (_lastType != selectedObject)
        {
            _lastType = selectedObject;
            UpdateMass();
        }
    }

    private void UpdateMass()
    {
        _rb.interpolation = RigidbodyInterpolation.Interpolate;

        if (objectMasses.TryGetValue(selectedObject, out float referenceMass))
        {
            Vector3 currentScale = transform.localScale;

            float mass = referenceMass * currentScale.x * currentScale.y * currentScale.z;

            _rb.mass = mass;
        }
    }
}