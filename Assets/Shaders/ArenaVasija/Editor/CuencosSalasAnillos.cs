using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Pone cuencos de fuego (los de la sala 3: pedestal + cuenco + fuego + su luz) a los lados del tablero
// en las salas de la Iguana y del Escarabajo. Herramienta de editor (Bauti).
public static class CuencosSalasAnillos
{
    const string Grupo = "Cuencos de fuego";
    const float Escala = 0.7f;

    static List<Transform> Escena() =>
        Resources.FindObjectsOfTypeAll<Transform>().Where(t => t != null && t.gameObject.scene.IsValid() && !EditorUtility.IsPersistent(t)).ToList();

    static Vector2 XZ(Vector3 v) => new Vector2(v.x, v.z);

    [MenuItem("Tepet/Anillos/Poner cuencos de fuego en Iguana y Escarabajo")]
    static void Poner()
    {
        try { PonerInterno(); } catch (System.Exception e) { Debug.LogError("[Cuencos] fallo: " + e); }
    }

    static void PonerInterno()
    {
        var todos = Escena();
        todos = todos.Where(t => !t.GetComponentsInParent<Transform>(true).Any(p => p.name == Grupo)).ToList();
        var cuenco = todos.FirstOrDefault(t => t.name == "Cuenco (1)" && t.parent && t.parent.name.StartsWith("IluminacionZonaPilares"));
        if (cuenco == null) { EditorUtility.DisplayDialog("Cuencos", "No encuentro el cuenco de referencia de la sala 3.", "OK"); return; }
        var sosten = todos.Where(t => t.name.StartsWith("Sosten de piramide High") && t.gameObject.activeInHierarchy)
                          .OrderBy(t => Vector2.Distance(XZ(t.position), XZ(cuenco.position))).First();
        var fuego = todos.Where(t => t.name.StartsWith("fireParticle2 cuenco"))
                         .OrderBy(t => Vector3.Distance(t.position, cuenco.position)).First();
        var luces = Object.FindObjectsByType<Light>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
                          .Where(l => l.type != LightType.Directional && Vector3.Distance(l.transform.position, cuenco.position) < 3f).ToList();
        var piezas = new List<Transform> { sosten, cuenco, fuego };
        piezas.AddRange(luces.Select(l => l.transform));

        float pisoRef = sosten.GetComponentsInChildren<Renderer>().Min(r => r.bounds.min.y);
        Vector3 origen = new Vector3(sosten.position.x, pisoRef, sosten.position.z);

        var salas = new (string sala, float xPared, float lado, float sep)[] { ("LizardPuzzleRoom", 139.3f, 1f, 1.0f), ("BeetlePuzzleRoom", 222.8f, -1f, 1.9f) };
        int n = 0;
        foreach (var (salaNombre, xPared, lado, sep) in salas)
        {
            var sala = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects().FirstOrDefault(r => r.name == salaNombre)?.transform;
            if (sala == null) { Debug.LogWarning("[Cuencos] no encuentro " + salaNombre); continue; }
            var viejo = sala.Find(Grupo); if (viejo) Undo.DestroyObjectImmediate(viejo.gameObject);
            var g = new GameObject(Grupo); Undo.RegisterCreatedObjectUndo(g, "Cuencos"); g.transform.SetParent(sala, true);
            foreach (float z in new[] { -100.3f, -104.7f })
            {
                float x = xPared + lado * sep;
                float piso = Physics.Raycast(new Vector3(x, -9f, z), Vector3.down, out var hit, 20f, ~0, QueryTriggerInteraction.Ignore) ? hit.point.y : -15.6f;
                var raiz = new GameObject("Cuenco " + (++n)); raiz.transform.SetParent(g.transform, false);
                raiz.transform.position = origen;
                foreach (var p in piezas) Copiar(p, raiz.transform);
                raiz.transform.localScale = Vector3.one * Escala;
                foreach (var l in raiz.GetComponentsInChildren<Light>(true)) { l.range *= Escala; l.lightmapBakeType = LightmapBakeType.Mixed; }
                raiz.transform.position = new Vector3(x, piso, z);
            }
        }
        EditorSceneManager.MarkSceneDirty(cuenco.gameObject.scene);
        Debug.Log($"[Cuencos] {n} cuencos puestos (escala {Escala}). Luces copiadas por cuenco: {luces.Count}" + (luces.Count > 0 ? $" ({luces[0].lightmapBakeType}). Si son Baked, hay que rehacer el bake." : ". No habia luz cerca del cuenco de referencia."));
    }

    static void Copiar(Transform src, Transform padre)
    {
        GameObject copia;
        if (PrefabUtility.GetOutermostPrefabInstanceRoot(src.gameObject) == src.gameObject)
        {
            copia = (GameObject)PrefabUtility.InstantiatePrefab(PrefabUtility.GetCorrespondingObjectFromSource(src.gameObject), padre);
            var mods = PrefabUtility.GetPropertyModifications(src.gameObject);
            if (mods != null) PrefabUtility.SetPropertyModifications(copia, mods);
        }
        else copia = Object.Instantiate(src.gameObject, padre);
        copia.name = src.name.Split(" (")[0];
        copia.transform.SetPositionAndRotation(src.position, src.rotation);
        copia.transform.localScale = src.lossyScale;
        copia.SetActive(true);
    }

    [MenuItem("Tepet/Anillos/Quitar cuencos de Iguana y Escarabajo")]
    static void Quitar()
    {
        foreach (var t in Escena().Where(t => t.name == Grupo && t.parent && (t.parent.name == "LizardPuzzleRoom" || t.parent.name == "BeetlePuzzleRoom")).ToList())
            Undo.DestroyObjectImmediate(t.gameObject);
    }
}
