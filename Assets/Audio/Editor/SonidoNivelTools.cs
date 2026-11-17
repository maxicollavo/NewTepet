using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Tepet > Sonido: arma los sonidos de la Sala de la Esfinge y de la Sala de los Anillos (Bauti).
public static class SonidoNivelTools
{
    const string Carpeta = "Assets/Resources/Sounds/";

    static Transform Buscar(string ruta)
    {
        var partes = ruta.Split('/');
        foreach (var root in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (root.name != partes[0]) continue;
            Transform t = root.transform;
            for (int i = 1; i < partes.Length && t != null; i++) t = t.Find(partes[i]);
            if (t != null) return t;
        }
        return null;
    }

    static AudioClip Clip(string nombre)
    {
        var c = AssetDatabase.LoadAssetAtPath<AudioClip>(Carpeta + nombre);
        if (c == null) Debug.LogWarning("[Sonido] no encuentro " + Carpeta + nombre);
        return c;
    }

    static GameObject Crear(Transform padre, string nombre, Vector3 posMundo, AudioClip clip, bool loop, float vol, float blend, float min, float max, AudioRolloffMode modo)
    {
        var go = new GameObject(nombre);
        Undo.RegisterCreatedObjectUndo(go, "Armar sonidos");
        go.transform.SetParent(padre, false);
        go.transform.position = posMundo;
        var s = go.AddComponent<AudioSource>();
        s.clip = clip; s.loop = loop; s.volume = vol; s.playOnAwake = false;
        s.spatialBlend = blend; s.minDistance = min; s.maxDistance = max; s.rolloffMode = modo; s.dopplerLevel = 0;
        go.AddComponent<SonidoNivel>();
        return go;
    }

    [MenuItem("Tepet/Sonido/Armar sonidos (Esfinge y Anillos)")]
    static void Armar()
    {
        Quitar(false);
        int n = 0;
        var sala = Buscar("Sala SUBTERRANEA");
        if (sala) { Crear(sala, "Sonido Ambiente Esfinge", new Vector3(180.9f, -30f, -127f), Clip("Niveles/Amb_Subterraneo_Loop.wav"), true, 0.5f, 1f, 14f, 26f, AudioRolloffMode.Linear); n++; }

        var cascada = sala ? sala.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "CascadaArena") : null;
        if (cascada == null) cascada = Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None).FirstOrDefault(t => t.name == "CascadaArena");
        if (cascada) { var p = cascada.position; Crear(cascada, "Sonido Cascada", new Vector3(p.x, -30f, p.z), Clip("Niveles/Cascada_Arena_Suave_Loop.wav"), true, 0.7f, 1f, 2f, 18f, AudioRolloffMode.Logarithmic); n++; }

        var ilum = sala ? sala.Find("Iluminacion Sala Esfinge") : null;
        var fuego = Clip("TorchFireLoop_Flaming Torch Sound for Sleeping _ Relaxing Fire Ambience-St.L.wav");
        if (ilum)
            foreach (Transform a in ilum)
            {
                var f = a.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name.StartsWith("fireParticle2"));
                Crear(a, "Sonido Fuego", f ? f.position : a.position + Vector3.up * 1.5f, fuego, true, 0.3f, 1f, 1f, 7f, AudioRolloffMode.Logarithmic); n++;
            }

        var anillos = Buscar("Nivel 4/Anillos");
        if (anillos) { Crear(anillos, "Sonido Ambiente Anillos", anillos.position + Vector3.up * 3f, Clip("Niveles/Amb_Anillos_Loop.wav"), true, 0.45f, 1f, 10f, 20f, AudioRolloffMode.Linear); n++; }

        var limites = Buscar("Nivel 4/Anillos/Limites");
        if (limites)
        {
            if (limites.gameObject.activeSelf) Debug.LogWarning("[Sonido] 'Limites' esta activo al arrancar: el sonido del ascensor sonaria al cargar. No lo agrego.");
            else
            {
                var s = Undo.AddComponent<AudioSource>(limites.gameObject);
                s.clip = Clip("Niveles/Ascensor_Bajando.wav"); s.loop = false; s.volume = 0.9f; s.playOnAwake = false; s.spatialBlend = 0f;
                Undo.AddComponent<SonidoNivel>(limites.gameObject); n++;
            }
        }
        EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        Debug.Log($"[Sonido] {n} sonidos armados (ambiente Esfinge, cascada, fuego de antorchas, ambiente Anillos, ascensor).");
    }

    [MenuItem("Tepet/Sonido/Quitar sonidos armados")]
    static void QuitarMenu() => Quitar(true);

    static void Quitar(bool log)
    {
        int n = 0;
        foreach (var s in Object.FindObjectsByType<SonidoNivel>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            if (s.gameObject.name.StartsWith("Sonido ")) { Undo.DestroyObjectImmediate(s.gameObject); n++; }
            else { var a = s.GetComponent<AudioSource>(); Undo.DestroyObjectImmediate(s); if (a) Undo.DestroyObjectImmediate(a); n++; }
        }
        if (n > 0) EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        if (log) Debug.Log($"[Sonido] {n} sonidos quitados.");
    }
}
