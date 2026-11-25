using UnityEditor;
using UnityEngine;

public static class VerEsferas
{
    [MenuItem("Tepet/zz Ver esferas")]
    static void Ver()
    {
        var sb = new System.Text.StringBuilder();
        foreach (var rs in Object.FindObjectsByType<RotateSphere>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            string path = rs.name; var p = rs.transform.parent; while (p) { path = p.name + "/" + path; p = p.parent; }
            var so = new SerializedObject(rs);
            sb.AppendLine($"ESFERA {path} pos={rs.transform.position} escala={rs.transform.lossyScale}");
            foreach (var n in new[] { "firstFillAmount", "secondFillAmount", "thirdFillAmount", "proximityTolerance", "shaderLerpDuration", "emissionIntensity" })
            { var pr = so.FindProperty(n); if (pr != null) sb.AppendLine($"   {n} = {pr.floatValue}"); }
            var w = so.FindProperty("winImagePoints"); if (w != null) sb.AppendLine($"   winImagePoints = {w.arraySize}");
            var a = so.FindProperty("allImages"); if (a != null) sb.AppendLine($"   allImages = {a.arraySize}");
            var pm = so.FindProperty("puzzleManager"); if (pm != null && pm.objectReferenceValue) { var m = (SpherePuzzleManager)pm.objectReferenceValue; sb.AppendLine($"   manager {m.name} nivel3={m.isOnLevelThree}"); }
            var child = rs.transform.childCount > 0 ? rs.transform.GetChild(0) : null;
            var r = child ? child.GetComponent<MeshRenderer>() : null;
            if (r)
            {
                var mats = r.sharedMaterials;
                for (int i = 0; i < mats.Length; i++)
                {
                    var m = mats[i]; if (!m) continue;
                    string f = m.HasProperty("_FillAmount") ? m.GetFloat("_FillAmount").ToString() : "-";
                    sb.AppendLine($"   mat[{i}] {m.name} shader={m.shader.name} _FillAmount={f} path={AssetDatabase.GetAssetPath(m)}");
                }
                var mf = child.GetComponent<MeshFilter>(); if (mf && mf.sharedMesh) sb.AppendLine($"   mesh {mf.sharedMesh.name} bounds {mf.sharedMesh.bounds} childScale={child.lossyScale}");
            }
        }
        foreach (var ms in Object.FindObjectsByType<MimicSphereActions>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            sb.AppendLine($"MIMIC {ms.name} offValue={ms.offValue}");
        System.IO.File.WriteAllText("ver_esferas.txt", sb.ToString());
        Debug.Log("[Esferas] ver_esferas.txt");
    }

    // La esfera del obelisco esta 0.33 m mas abajo que la del nivel 1: mismos valores corridos.
    [MenuItem("Tepet/zz Arreglar esfera del obelisco")]
    static void Arreglar()
    {
        var mat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Shaders/Level3InteractSphere.mat");
        if (mat) { Undo.RecordObject(mat, "Esfera"); mat.SetFloat("_Fill", 0.075f); EditorUtility.SetDirty(mat); }
        foreach (var rs in Object.FindObjectsByType<RotateSphere>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            var so = new SerializedObject(rs);
            var pm = so.FindProperty("puzzleManager").objectReferenceValue as SpherePuzzleManager;
            if (pm == null || !pm.isOnLevelThree) continue;
            so.FindProperty("firstFillAmount").floatValue = 0.5f;
            so.FindProperty("secondFillAmount").floatValue = 1.03f;
            so.FindProperty("thirdFillAmount").floatValue = 1.7f;
            so.FindProperty("emptyFillAmount").floatValue = -3.3f;
            so.ApplyModifiedProperties();
            Debug.Log("[Esferas] arreglada " + rs.name);
        }
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        AssetDatabase.SaveAssets();
    }
}
