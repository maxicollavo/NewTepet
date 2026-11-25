using UnityEditor;
using UnityEngine;

public static class RevisarAlfaCuencos
{
    [MenuItem("Tepet/Vasijas/zz Revisar transparencia de los cuencos")]
    static void Revisar()
    {
        string carpeta = "Assets/Art/Assets/EgyptAssets/Pressure Plate/Cuencos Arena/Texturas/";
        var sb = new System.Text.StringBuilder();
        foreach (var n in new[] { "Cuenco 1", "Cuenco 1 Var", "Cuenco 2", "Cuenco 3" })
        {
            string png = carpeta + n + "/" + n + "_initialShadingGroup_AlbedoTransparency.png";
            var tex = new Texture2D(2, 2);
            if (!System.IO.File.Exists(png) || !tex.LoadImage(System.IO.File.ReadAllBytes(png))) { sb.AppendLine(n + ": no la pude leer"); continue; }
            var px = tex.GetPixels32(); int bajos = 0; byte min = 255;
            foreach (var p in px) { if (p.a < 128) bajos++; if (p.a < min) min = p.a; }
            sb.AppendLine($"{n}: alfa minimo {min}, {100f * bajos / px.Length:F2}% transparente");
            Object.DestroyImmediate(tex);
        }
        Debug.Log("[Cuencos] " + sb.ToString().Replace("\n", " | "));
    }
}
