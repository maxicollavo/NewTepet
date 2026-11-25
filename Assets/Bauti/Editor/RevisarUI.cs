using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public static class RevisarUI
{
    [MenuItem("Tepet/Revisar UI de puzzles")]
    static void Revisar()
    {
        var sb = new StringBuilder();
        var todos = Resources.FindObjectsOfTypeAll<Transform>().Where(t => t.gameObject.scene.IsValid() && !EditorUtility.IsPersistent(t)).ToList();
        foreach (var r in todos.Where(t => t.parent == null && t.name.ToLower().StartsWith("tutorial")))
        {
            sb.AppendLine($"== {r.name} activo={r.gameObject.activeSelf}");
            foreach (var img in r.GetComponentsInChildren<Image>(true))
            {
                var rt = (RectTransform)img.transform;
                sb.AppendLine($"   {img.name} sprite={(img.sprite ? AssetDatabase.GetAssetPath(img.sprite) : "-")} pos={rt.anchoredPosition} size={rt.sizeDelta} scale={rt.localScale} anchorMin={rt.anchorMin} anchorMax={rt.anchorMax} parent={rt.parent.name}");
            }
            // quien lo referencia
            foreach (var mb in Object.FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (mb == null) continue;
                var so = new SerializedObject(mb); var it = so.GetIterator();
                while (it.NextVisible(true))
                    if (it.propertyType == SerializedPropertyType.ObjectReference && it.objectReferenceValue != null &&
                        (it.objectReferenceValue == r.gameObject || it.objectReferenceValue == r))
                        sb.AppendLine($"   usado por {mb.GetType().Name} en {mb.name} ({it.propertyPath})");
            }
        }
        System.IO.File.WriteAllText(Application.dataPath + "/../revisar_ui.txt", sb.ToString());
        Debug.Log("UI revisada");
    }
}
