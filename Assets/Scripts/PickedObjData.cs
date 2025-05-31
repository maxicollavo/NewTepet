using System.Collections.Generic;
using UnityEngine;

public class PickedObjData : MonoBehaviour
{
    public static PickedObjData Instance;

    private Dictionary<ObjectsToPick, bool> pickedObjects = new Dictionary<ObjectsToPick, bool>();

    private ObjectsToPick currentPickedObj = ObjectsToPick.None;
    //Consultar a esta variable para saber el objeto que se tiene en la mano
    public ObjectsToPick CurrentPickedObj => currentPickedObj;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            foreach (ObjectsToPick obj in System.Enum.GetValues(typeof(ObjectsToPick)))
            {
                pickedObjects[obj] = false;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Llamar a este metodo cuando un objeto sea recogido
    public void MarkAsPicked(ObjectsToPick obj)
    {
        if (pickedObjects.ContainsKey(obj))
        {
            pickedObjects[obj] = true;
            currentPickedObj = obj;
            HandInventory.Instance.ShowObjectInHand(currentPickedObj);
        }
    }

    //Llamar a este metodo cuando se suelte un objeto
    public void MarkAsThrowed(ObjectsToPick obj)
    {
        if (pickedObjects.ContainsKey(obj))
        {
            pickedObjects[obj] = false;
            HandInventory.Instance.DisableObjectInHand(currentPickedObj);

            if (currentPickedObj == obj)
            {
                currentPickedObj = ObjectsToPick.None;
            }
        }
    }

    // Llamar a este metodo desde otros scripts para preguntar si ya fue recogido
    public bool WasPicked(ObjectsToPick obj)
    {
        return pickedObjects.TryGetValue(obj, out bool picked) && picked;
    }

    public Dictionary<ObjectsToPick, bool> GetPickedObjects()
    {
        return pickedObjects;
    }
}
