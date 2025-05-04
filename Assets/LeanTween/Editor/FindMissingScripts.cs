using UnityEngine;
using UnityEditor;

public class FindMissingScripts : MonoBehaviour
{
    [MenuItem("Tools/Find Missing Scripts in Scene")]
    [System.Obsolete]
    public static void Find()
    {
        GameObject[] objects = GameObject.FindObjectsOfType<GameObject>();
        int count = 0;
        foreach (GameObject go in objects)
        {
            Component[] components = go.GetComponents<Component>();
            for (int i = 0; i < components.Length; i++)
            {
                if (components[i] == null)
                {
                    Debug.Log($"Missing script on: {go.name}", go);
                    count++;
                }
            }
        }
        Debug.Log($"Total GameObjects with missing scripts: {count}");
    }
}