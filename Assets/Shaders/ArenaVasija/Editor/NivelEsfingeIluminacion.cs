using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Iluminacion de la Sala de la Esfinge usando las antorchas de piso doradas que ya existen
// (modelo "Antorcha de piso Substance dorada" + "fireParticle2 cuenco" + su luz). Solo herramienta de editor (Bauti).
public static class NivelEsfingeIluminacion
{
    const string Grupo = "Iluminacion Sala Esfinge";
    const float YPiso = -34.2f;

    // (x, z) en mundo. Sala: x 164.1..197.8, z -141.0..-112.2, nicho de la esfinge x 173.7..183.3 hasta z -145.8
    static readonly Vector2[] Puntos =
    {
        new Vector2(177.3f, -114.6f), new Vector2(184.6f, -114.6f),   // salida del tunel
        new Vector2(177.3f, -124.0f), new Vector2(184.6f, -124.0f),   // camino central
        new Vector2(172.2f, -139.6f), new Vector2(184.9f, -139.6f),   // flanquean el nicho de la esfinge
        new Vector2(166.0f, -139.4f), new Vector2(196.0f, -139.4f),   // esquinas del fondo
        new Vector2(196.0f, -114.0f),                                 // esquina noreste (la noroeste ya tiene la luz del pozo)
    };

    static List<Transform> Escena() =>
        Resources.FindObjectsOfTypeAll<Transform>().Where(t => t.gameObject.scene.IsValid() && !EditorUtility.IsPersistent(t)).ToList();

    [MenuItem("Tepet/Nivel Esfinge/Armar iluminacion (antorchas de piso)")]
    static void Armar()
    {
        var todos = Escena();
        var sala = todos.FirstOrDefault(t => t.name == "Sala SUBTERRANEA");
        var grupoRef = todos.FirstOrDefault(t => t.name == "antorchas de piso");
        if (sala == null || grupoRef == null) { EditorUtility.DisplayDialog("Nivel Esfinge", "No encuentro 'Sala SUBTERRANEA' o 'antorchas de piso'.", "OK"); return; }
        if (sala.Find(Grupo) != null) { EditorUtility.DisplayDialog("Nivel Esfinge", "Ya hay iluminacion. Usa 'Quitar iluminacion' primero.", "OK"); return; }

        var torch = grupoRef.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name.StartsWith("Antorcha de piso Substance dorada"));
        if (torch == null) { Debug.LogWarning("[Nivel Esfinge] no hay antorcha de referencia"); return; }
        var fuego = grupoRef.GetComponentsInChildren<Transform>(true).Where(t => t.name.StartsWith("fireParticle2"))
                    .OrderBy(t => Vector3.Distance(t.position, torch.position)).FirstOrDefault();
        var luz = Object.FindObjectsByType<Light>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                    .Where(l => l.type != LightType.Directional)
                    .OrderBy(l => Vector3.Distance(l.transform.position, (fuego ? fuego : torch).position)).FirstOrDefault();
        float dLuz = luz ? Vector3.Distance(luz.transform.position, (fuego ? fuego : torch).position) : 999f;
        if (dLuz > 3f) luz = null;

        // posiciones a evitar (placas, vasijas, esfinge, cascada)
        var evitar = todos.Where(t => t.IsChildOf(sala) && System.Text.RegularExpressions.Regex.IsMatch(t.name, "placa|plate|vasija|vase|cascada|SandCascade", System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                          .Select(t => new Vector2(t.position.x, t.position.z)).ToList();

        var g = new GameObject(Grupo);
        Undo.RegisterCreatedObjectUndo(g, "Armar iluminacion");
        g.transform.SetParent(sala, true);

        int i = 0;
        foreach (var p0 in Puntos)
        {
            var p = p0;
            foreach (var e in evitar) { var d = p - e; if (d.magnitude < 1.8f) p = e + (d.sqrMagnitude < 1e-4f ? Vector2.right : d.normalized) * 1.8f; }

            var raiz = new GameObject("Antorcha " + (++i));
            raiz.transform.SetParent(g.transform, false);
            raiz.transform.position = torch.position;
            Copiar(torch, raiz.transform);
            if (fuego) Copiar(fuego, raiz.transform);
            if (luz) Copiar(luz.transform, raiz.transform);

            raiz.transform.position = new Vector3(p.x, torch.position.y, p.y);
            var rs = raiz.GetComponentsInChildren<Renderer>(true).Where(r => !(r is ParticleSystemRenderer)).ToArray();
            if (rs.Length > 0)
            {
                float minY = rs.Min(r => r.bounds.min.y);
                raiz.transform.position += Vector3.up * (YPiso - minY);
            }
        }
        EditorSceneManager.MarkSceneDirty(sala.gameObject.scene);
        Selection.activeGameObject = g;
        Debug.Log($"[Nivel Esfinge] {i} antorchas de piso armadas. Referencia: '{torch.name}', fuego: '{(fuego ? fuego.name : "-")}', luz: '{(luz ? luz.name + " (" + luz.lightmapBakeType + ", int " + luz.intensity + ", rango " + luz.range + ")" : "ninguna a menos de 3 m")}'. Si las luces son Baked, rehacer el bake de iluminacion.");
    }

    static void Copiar(Transform src, Transform padre)
    {
        GameObject copia;
        var raizPrefab = PrefabUtility.GetOutermostPrefabInstanceRoot(src.gameObject);
        if (raizPrefab == src.gameObject)
        {
            var fuente = PrefabUtility.GetCorrespondingObjectFromSource(src.gameObject);
            copia = (GameObject)PrefabUtility.InstantiatePrefab(fuente, padre);
            var mods = PrefabUtility.GetPropertyModifications(src.gameObject);
            if (mods != null) PrefabUtility.SetPropertyModifications(copia, mods);
        }
        else copia = Object.Instantiate(src.gameObject, padre);
        copia.name = src.name.Split(" (")[0];
        copia.transform.SetPositionAndRotation(src.position, src.rotation);
        copia.transform.localScale = Vector3.Scale(src.lossyScale, new Vector3(1f / padre.lossyScale.x, 1f / padre.lossyScale.y, 1f / padre.lossyScale.z));
        copia.SetActive(true);
    }

    [MenuItem("Tepet/Nivel Esfinge/Quitar iluminacion")]
    static void Quitar()
    {
        var sala = Escena().FirstOrDefault(t => t.name == "Sala SUBTERRANEA");
        var g = sala ? sala.Find(Grupo) : null;
        if (g) { Undo.DestroyObjectImmediate(g.gameObject); EditorSceneManager.MarkSceneDirty(sala.gameObject.scene); }
    }
}
