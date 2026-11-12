// Guarda la vista actual de la Scene view como encuadre para el trailer (TrailerCaptures/encuadres.json).
using System.IO; using UnityEditor; using UnityEngine;
public static class TrailerEncuadres
{
    static string Ruta => Path.GetFullPath(Path.Combine(Application.dataPath, "..", "TrailerCaptures", "encuadres.json"));
    static void Guardar(string nombre)
    {
        var sv = SceneView.lastActiveSceneView;
        if (sv == null) { EditorUtility.DisplayDialog("Trailer", "Abri la Scene view y pone la vista donde queres.", "OK"); return; }
        var lista = File.Exists(Ruta) ? JsonUtility.FromJson<TrailerDrone.PoseList>(File.ReadAllText(Ruta)) : new TrailerDrone.PoseList();
        lista.items.RemoveAll(p => p.name == nombre);
        var cam = sv.camera.transform;
        lista.items.Add(new TrailerDrone.Pose { name = nombre, pos = cam.position, rot = cam.rotation, fov = sv.cameraSettings.fieldOfView });
        File.WriteAllText(Ruta, JsonUtility.ToJson(lista, true));
        Debug.Log($"[Trailer] encuadre '{nombre}' guardado en {cam.position}");
    }
    [MenuItem("Tepet/Trailer/Guardar encuadre/Cuencos - Inicio")] static void C1() => Guardar("cuencos_inicio");
    [MenuItem("Tepet/Trailer/Guardar encuadre/Cuencos - Fin")] static void C2() => Guardar("cuencos_fin");
    [MenuItem("Tepet/Trailer/Guardar encuadre/Mural - Inicio")] static void M1() => Guardar("mural_inicio");
    [MenuItem("Tepet/Trailer/Guardar encuadre/Mural - Fin")] static void M2() => Guardar("mural_fin");
}
