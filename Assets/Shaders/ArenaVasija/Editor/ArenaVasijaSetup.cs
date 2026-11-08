// Tepet's Pyramid - Agrega el relleno de arena a todas las vasijas de la escena (mundo y mano).
// Menu: Tepet > Vasijas > Agregar arena a las vasijas. Se puede deshacer con Ctrl+Z.
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class ArenaVasijaSetup
{
    const string ChildName = "ArenaRelleno";
    const string MatPath = "Assets/Shaders/ArenaVasija/M_ArenaVasija.mat";

    [MenuItem("Tepet/Vasijas/Agregar arena a las vasijas")]
    static void Agregar()
    {
        var mat = AssetDatabase.LoadAssetAtPath<Material>(MatPath);
        if (mat == null) { EditorUtility.DisplayDialog("Arena vasijas", "No encuentro " + MatPath, "OK"); return; }
        int hechas = 0, saltadas = 0;
        foreach (var t in Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            bool esVasija = t.GetComponent("Vase") != null || t.GetComponent("HandVases") != null;
            if (!esVasija) continue;
            if (t.Find(ChildName) != null) { saltadas++; continue; }
            Crear(t, mat);
            hechas++;
        }
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log($"[Arena vasijas] agregado a {hechas} vasijas ({saltadas} ya lo tenian). Guarda la escena para conservarlo.");
    }

    [MenuItem("Tepet/Vasijas/Quitar arena de las vasijas")]
    static void Quitar()
    {
        int n = 0;
        foreach (var t in Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            if (t != null && t.name == ChildName && t.GetComponent<VasijaArenaVisual>() != null) { Undo.DestroyObjectImmediate(t.gameObject); n++; }
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log($"[Arena vasijas] quitado de {n} vasijas.");
    }

    static void Crear(Transform vasija, Material mat)
    {
        // limites de la vasija en su propio espacio
        Bounds b = new Bounds(Vector3.zero, Vector3.zero); bool tiene = false;
        foreach (var mf in vasija.GetComponentsInChildren<MeshFilter>(true))
        {
            if (mf.sharedMesh == null || mf.name == ChildName) continue;
            var mb = mf.sharedMesh.bounds;
            for (int i = 0; i < 8; i++)
            {
                var c = mb.center + Vector3.Scale(mb.extents, new Vector3((i & 1) == 0 ? -1 : 1, (i & 2) == 0 ? -1 : 1, (i & 4) == 0 ? -1 : 1));
                var p = vasija.InverseTransformPoint(mf.transform.TransformPoint(c));
                if (!tiene) { b = new Bounds(p, Vector3.zero); tiene = true; } else b.Encapsulate(p);
            }
        }
        if (!tiene) b = new Bounds(Vector3.zero, Vector3.one);

        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Undo.RegisterCreatedObjectUndo(go, "Arena vasija");
        go.name = ChildName;
        Object.DestroyImmediate(go.GetComponent<Collider>());   // que no tape el agarre ni la interaccion
        go.layer = vasija.gameObject.layer;
        go.transform.SetParent(vasija, false);
        // PRUEBA con los corazones: un cubo un poco mas grande que la vasija, asi la arena se ve subir por afuera.
        // Con los modelos reales: usar la forma interior de la vasija (mas chica que la vasija).
        go.transform.localPosition = b.center;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = b.size * 1.1f;
        var r = go.GetComponent<MeshRenderer>();
        r.sharedMaterial = mat;
        if (vasija.GetComponent("HandVases") != null) r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        var v = go.AddComponent<VasijaArenaVisual>();
        v.llenado = 0f;
    }
}
