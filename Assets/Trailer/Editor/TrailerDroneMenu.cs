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
}
