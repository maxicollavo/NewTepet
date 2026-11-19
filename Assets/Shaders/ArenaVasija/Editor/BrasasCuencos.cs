using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

// Pone brasas dentro de los cuencos de fuego de las salas de la Iguana y del Escarabajo,
// para que no se vea el cuenco vacio por dentro.
public static class BrasasCuencos
{
    const string Carpeta = "Assets/Art/Assets/Brasas/";
    const string Nombre = "Brasas";

    static Material Material()
    {
        string ruta = Carpeta + "BrasasMat.mat";
        var m = AssetDatabase.LoadAssetAtPath<Material>(ruta);
        if (m != null) return m;
        m = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        m.SetTexture("_BaseMap", AssetDatabase.LoadAssetAtPath<Texture2D>(Carpeta + "brasas_color.png"));
        m.SetFloat("_Smoothness", 0.15f);
        m.EnableKeyword("_EMISSION");
        m.SetTexture("_EmissionMap", AssetDatabase.LoadAssetAtPath<Texture2D>(Carpeta + "brasas_emission.png"));
        m.SetColor("_EmissionColor", new Color(1f, 0.55f, 0.25f) * 3f);
        m.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        AssetDatabase.CreateAsset(m, ruta);
        return m;
    }

    [MenuItem("Tepet/Anillos/Poner brasas en los cuencos")]
    static void Poner()
    {
        var mesh = AssetDatabase.LoadAllAssetsAtPath(Carpeta + "brasas.obj").OfType<Mesh>().FirstOrDefault();
        if (mesh == null) { EditorUtility.DisplayDialog("Brasas", "No encuentro brasas.obj", "OK"); return; }
        var mat = Material();
        Quitar();
        int n = 0;
        foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (root.name != "LizardPuzzleRoom" && root.name != "BeetlePuzzleRoom") continue;
            var grupo = root.transform.Find("Cuencos"); if (grupo == null) continue;
            foreach (var r in grupo.GetComponentsInChildren<MeshRenderer>(true).Where(r => r != null && r.name.StartsWith("Cuenco")).ToList())
            {
                var b = r.bounds;
                float radio = Mathf.Min(b.extents.x, b.extents.z) * 0.6f;
                var go = new GameObject(Nombre);
                Undo.RegisterCreatedObjectUndo(go, "Brasas");
                go.transform.SetParent(r.transform.parent, true);
                go.transform.position = new Vector3(b.center.x, b.max.y - b.size.y * 0.24f, b.center.z);
                go.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
                go.transform.localScale = Vector3.one;
                var s = go.transform.lossyScale;
                go.transform.localScale = new Vector3(radio / s.x, radio / s.y, radio / s.z);
                go.AddComponent<MeshFilter>().sharedMesh = mesh;
                var mr = go.AddComponent<MeshRenderer>(); mr.sharedMaterial = mat; mr.shadowCastingMode = ShadowCastingMode.Off;
                n++;
            }
        }
        EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        Debug.Log($"[Brasas] brasas puestas en {n} cuencos.");
    }

    [MenuItem("Tepet/Anillos/Quitar brasas de los cuencos")]
    static void Quitar()
    {
        foreach (var t in Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None).Where(t => t.name == Nombre && t.GetComponent<MeshFilter>()).ToList())
            Undo.DestroyObjectImmediate(t.gameObject);
    }
}
