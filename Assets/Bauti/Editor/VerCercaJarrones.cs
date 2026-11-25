using System.Linq;
using UnityEditor;
using UnityEngine;

public static class VerCercaJarrones
{
    [MenuItem("Tepet/Vasijas/zz Ver objetos cerca de los jarrones")]
    static void Ver()
    {
        var vs = Object.FindObjectsByType<Vase>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (vs.Length == 0) return;
        Vector3 c = Vector3.zero; foreach (var v in vs) c += v.transform.position; c /= vs.Length;
        var sb = new System.Text.StringBuilder($"centro {c}\n");
        foreach (var v in vs) sb.AppendLine($"VASE {v.name} idx{v.vaseIndex} pos {v.transform.position}");
        foreach (var r in Object.FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (r.GetComponentInParent<Vase>(true) || r.GetComponentInParent<HandVases>(true)) continue;
            var mf = r.GetComponent<MeshFilter>(); if (!mf || !mf.sharedMesh) continue;
            float d = Vector3.Distance(r.bounds.center, c);
            if (d > 12f) continue;
            string path = r.name; var p = r.transform.parent; while (p) { path = p.name + "/" + path; p = p.parent; }
            sb.AppendLine($"{d:F1}m  {path}  mesh={mf.sharedMesh.name}  mat={(r.sharedMaterial ? r.sharedMaterial.name : "-")}  pos={r.bounds.center}  size={r.bounds.size}  activo={r.gameObject.activeInHierarchy}");
        }
        System.IO.File.WriteAllText("cerca_jarrones.txt", sb.ToString());
        Debug.Log("[Jarrones] listado en cerca_jarrones.txt");
    }
}
