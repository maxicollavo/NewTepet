// Exporta posiciones (centro de bounds) de los objetos de la escena hasta 3 niveles, para planear tomas del trailer.
using System.IO; using System.Linq; using System.Text;
using UnityEditor; using UnityEngine;
public static class TrailerMapa
{
    [MenuItem("Tepet/Trailer/Exportar mapa de la escena")]
    static void Exportar()
    {
        var sb = new StringBuilder();
        foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
            Dump(root.transform, 0, sb);
        var p = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "TrailerCaptures", "salas.txt"));
        File.WriteAllText(p, sb.ToString());
        Debug.Log("[Trailer] mapa exportado: " + p);
    }
    static void Dump(Transform t, int depth, StringBuilder sb)
    {
        var rs = t.GetComponentsInChildren<Renderer>(true).Where(r => !(r is ParticleSystemRenderer)).ToArray();
        string b = "";
        if (rs.Length > 0) { var bb = rs[0].bounds; foreach (var r in rs) bb.Encapsulate(r.bounds); b = $" c({bb.center.x:F1},{bb.center.y:F1},{bb.center.z:F1}) s({bb.size.x:F1},{bb.size.y:F1},{bb.size.z:F1})"; }
        var cams = t.GetComponents<Component>().Any(c => c != null && c.GetType().Name.StartsWith("Cinemachine")) ? " [CAM]" : "";
        sb.AppendLine($"{new string(' ', depth * 2)}{t.name}{(t.gameObject.activeInHierarchy ? "" : " (off)")} p({t.position.x:F1},{t.position.y:F1},{t.position.z:F1}){b}{cams}");
        if (depth < 3) foreach (Transform c in t) Dump(c, depth + 1, sb);
    }
}
