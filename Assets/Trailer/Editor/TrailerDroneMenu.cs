// Tepet's Pyramid - menu para grabar las tomas del trailer con TrailerDrone.
using UnityEditor;
using UnityEngine;
using System.IO;

public static class TrailerDroneMenu
{
    static string Root => Path.GetFullPath(Path.Combine(Application.dataPath, "..", "TrailerCaptures"));

    [MenuItem("Tepet/Trailer/Previsualizar tomas (3 fotos por toma)")]
    static void Preview() => Launch(1);

    [MenuItem("Tepet/Trailer/Grabar tomas (video completo)")]
    static void Record() => Launch(2);

    [MenuItem("Tepet/Trailer/Abrir carpeta de capturas")]
    static void OpenFolder()
    {
        Directory.CreateDirectory(Root);
        EditorUtility.RevealInFinder(Root);
    }

    static void Launch(int mode)
    {
        if (!File.Exists(Path.Combine(Root, "shots.json")))
        {
            EditorUtility.DisplayDialog("Trailer", "Falta TrailerCaptures/shots.json", "OK");
            return;
        }
        if (EditorApplication.isPlaying) return;
        EditorPrefs.SetInt("TEPET_TRAILER_MODE", mode);
        EditorApplication.isPlaying = true;
    }

    // Para grabar gameplay jugando (con NVIDIA ShadowPlay u OBS): en Play, oculta/muestra la UI.
    // No toca nada guardado: al salir de Play vuelve todo como estaba.
    static readonly System.Collections.Generic.List<Behaviour> ocultos = new System.Collections.Generic.List<Behaviour>();

    [MenuItem("Tepet/Trailer/Ocultar o mostrar UI mientras juego (Play) %#u")]
    static void ToggleUI()
    {
        if (!EditorApplication.isPlaying) { EditorUtility.DisplayDialog("Trailer", "Funciona en Play: entra a jugar y usa Ctrl+Shift+U.", "OK"); return; }
        if (ocultos.Count > 0)
        {
            foreach (var b in ocultos) if (b) b.enabled = true;
            ocultos.Clear();
            Debug.Log("[Trailer] UI visible");
            return;
        }
        foreach (var c in Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None))
            if (c.enabled && c.isRootCanvas) { c.enabled = false; ocultos.Add(c); }
        Debug.Log($"[Trailer] UI oculta ({ocultos.Count} canvas). Ctrl+Shift+U para volver.");
    }
}
