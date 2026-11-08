// Tepet's Pyramid - Camara "dron" para grabar el trailer con gameplay real.
// No modifica ningun script del juego: solo mueve una camara propia, llama metodos
// existentes por reflexion (bajar el obelisco, girar anillos, prender antorchas...) y graba cuadros.
// Las tomas se definen en <proyecto>/TrailerCaptures/shots.json
// Se lanza desde el menu Tepet > Trailer (ver Editor/TrailerDroneMenu.cs).
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class TrailerDrone : MonoBehaviour
{
    [Serializable] public class Ev
    {
        public float time;            // segundo dentro de la toma
        public string target;         // ruta del objeto (vacio = objetivo de la toma)
        public string component;      // nombre del tipo (ej: ObeliskOnWin)
        public string method;         // metodo a llamar (publico o privado)
        public float[] args;          // argumentos numericos (float/int/bool)
        public bool all;              // llamar en todos los componentes de ese tipo bajo el objeto
        public float stagger = 0.4f;  // separacion entre llamadas cuando all = true
        public string activate;       // ruta de objeto a activar en ese momento
    }

    [Serializable] public class Shot
    {
        public string name;
        public string target;                 // ruta del objeto al que mira la toma
        public float duration = 5f;
        public string anchor;
        public float push, slide, rise, yaw;
        public string aimAt;
        public float[] lookAt;
        public string move = "orbit";         // orbit | path | static
        public float startAngle = -20, endAngle = 20;     // orbit: grados alrededor del objetivo
        public float startDist = 1.6f, endDist = 1.3f;    // en multiplos del radio del objetivo
        public float startHeight = 0.4f, endHeight = 0.3f;
        public float[] from, to;              // path: posicion relativa al centro (en radios)
        public float[] lookOffset;            // desplazamiento del punto de mira (en radios)
        public float fov = 40f;
        public float radiusOverride;          // forzar radio (metros) si el objeto es raro
        public bool skip;
        public Ev[] events;
    }

    [Serializable] public class ShotList
    {
        public string[] hide;
        public int fps = 30;
        public float warmup = 2f;
        public int jpgQuality = 92;
        public string[] only;                 // si tiene nombres, graba solo esas tomas
        public Shot[] shots;
    }

    bool loggedPink;
    public static int Mode;                   // 1 = previsualizar (3 fotos por toma), 2 = grabar
    static string Root => Path.GetFullPath(Path.Combine(Application.dataPath, "..", "TrailerCaptures"));

    Camera cam;
    ShotList list;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Boot()
    {
#if UNITY_EDITOR
        int m = UnityEditor.EditorPrefs.GetInt("TEPET_TRAILER_MODE", 0);
        if (m == 0) return;
        UnityEditor.EditorPrefs.SetInt("TEPET_TRAILER_MODE", 0);
        Mode = m;
        new GameObject("TrailerDrone").AddComponent<TrailerDrone>();
#endif
    }

    IEnumerator Start()
    {
        DontDestroyOnLoad(gameObject);
        string json = File.ReadAllText(Path.Combine(Root, "shots.json"));
        list = JsonUtility.FromJson<ShotList>(json);
        File.WriteAllText(Path.Combine(Root, "log.txt"), "inicio " + DateTime.Now + "\n");

        yield return new WaitForSecondsRealtime(list.warmup);
        SetupCamera();

        foreach (var shot in list.shots)
        {
            if (shot.skip) continue;
            if (list.only != null && list.only.Length > 0 && Array.IndexOf(list.only, shot.name) < 0) continue;
            yield return RunShot(shot);
        }
        Time.captureFramerate = 0;
        Log("fin");
        File.WriteAllText(Path.Combine(Root, "done.txt"), DateTime.Now.ToString());
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    void SetupCamera()
    {
        Camera main = Camera.main;
        var go = new GameObject("TrailerCam");
        cam = go.AddComponent<Camera>();
        if (main != null)
        {
            cam.CopyFrom(main);
            var srcData = main.GetComponent<UniversalAdditionalCameraData>();
            var data = go.AddComponent<UniversalAdditionalCameraData>();
            data.renderPostProcessing = true;
            if (srcData != null)
            {
                data.antialiasing = srcData.antialiasing;
                data.antialiasingQuality = srcData.antialiasingQuality;
                data.volumeLayerMask = srcData.volumeLayerMask;
                data.renderShadows = srcData.renderShadows;
            }
            main.enabled = false;
            var al = main.GetComponent<AudioListener>(); if (al) al.enabled = false;
        }
        go.AddComponent<AudioListener>();
        cam.depth = 100;
        cam.nearClipPlane = 0.03f;
        DontDestroyOnLoad(go);
    }

    void HideUI()
    {
        foreach (var c in FindObjectsByType<Canvas>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            c.enabled = false;
        foreach (var r in FindObjectsByType<MeshRenderer>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            foreach (var comp in r.GetComponents<Component>())
                if (comp != null && comp.GetType().Name == "TextMeshPro") { r.enabled = false; break; }
        foreach (var n in new[] { "FPSController", "PlayerInputHandler" })
        {
            var p = FindPath(n);
            if (p) foreach (var r in p.GetComponentsInChildren<Renderer>(true)) r.enabled = false;
        }
        foreach (var hm in FindObjectsByType<MonoBehaviour>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            if (hm != null && (hm.GetType().Name == "HandsManager" || hm.GetType().Name == "HandInventory"))
                foreach (var r in hm.GetComponentsInChildren<Renderer>(true)) r.enabled = false;
        int pink = 0;
        foreach (var r in FindObjectsByType<Renderer>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
        {
            if (!r.enabled || r is ParticleSystemRenderer) continue;
            foreach (var m in r.sharedMaterials)
                if (m == null || m.shader == null || !m.shader.isSupported) { r.enabled = false; pink++; break; }
        }
        if (pink > 0 && !loggedPink) { Log("ocultos por material faltante: " + pink); loggedPink = true; }
        if (list != null && list.hide != null)
            foreach (var h in list.hide) { var g = FindPath(h); if (g) g.SetActive(false); }
    }

    // ---------- busqueda de objetos (incluye inactivos) ----------
    static GameObject FindPath(string path)
    {
        if (string.IsNullOrEmpty(path)) return null;
        string[] parts = path.Split('/');
        for (int s = 0; s < SceneManager.sceneCount; s++)
        {
            foreach (var root in SceneManager.GetSceneAt(s).GetRootGameObjects())
            {
                if (root.name != parts[0]) continue;
                Transform t = root.transform;
                bool ok = true;
                for (int i = 1; i < parts.Length && ok; i++)
                {
                    Transform next = null;
                    foreach (Transform ch in t) if (ch.name == parts[i]) { next = ch; break; }
                    if (next == null) ok = false; else t = next;
                }
                if (ok) return t.gameObject;
            }
        }
        // si no se encontro por ruta, buscar solo por nombre
        string last = parts[parts.Length - 1];
        foreach (var t in Resources.FindObjectsOfTypeAll<Transform>())
            if (t.name == last && t.gameObject.scene.IsValid()) return t.gameObject;
        return null;
    }

    static void EnsureActive(GameObject go)
    {
        for (Transform t = go.transform; t != null; t = t.parent)
            if (!t.gameObject.activeSelf) t.gameObject.SetActive(true);
    }

    static Bounds GetBounds(GameObject go)
    {
        var rs = go.GetComponentsInChildren<Renderer>(true);
        bool has = false; Bounds b = new Bounds(go.transform.position, Vector3.one);
        foreach (var r in rs)
        {
            if (r is ParticleSystemRenderer) continue;
            if (!has) { b = r.bounds; has = true; } else b.Encapsulate(r.bounds);
        }
        return b;
    }

    static Vector3 V(float[] a, Vector3 def) => (a != null && a.Length == 3) ? new Vector3(a[0], a[1], a[2]) : def;
    static float Ease(float t) { t = Mathf.Clamp01(t); return t * t * (3 - 2 * t); }

    void Log(string s) => File.AppendAllText(Path.Combine(Root, "log.txt"), s + "\n");

    // ---------- toma ----------
    IEnumerator RunShot(Shot shot)
    {
        GameObject target = FindPath(shot.target);
        if (target == null) { Log("NO ENCONTRADO: " + shot.name + " -> " + shot.target); yield break; }
        EnsureActive(target);
        HideUI();
        yield return null;

        Transform anc = null; float ancFov = shot.fov;
        if (shot.move == "anchor")
        {
            var ag = FindPath(string.IsNullOrEmpty(shot.anchor) ? shot.target : shot.anchor);
            if (ag == null) { Log("ANCLA NO ENCONTRADA: " + shot.anchor); yield break; }
            anc = ag.transform; ancFov = AnchorFov(ag, shot.fov);
        }
        Vector3? aimPoint = null;
        if (anc != null && !string.IsNullOrEmpty(shot.aimAt))
        {
            var ag2 = FindPath(shot.aimAt);
            if (ag2 != null) { EnsureActive(ag2); aimPoint = GetBounds(ag2).center + V(shot.lookOffset, Vector3.zero); }
            else Log("aimAt no encontrado: " + shot.aimAt);
        }
        Bounds b = GetBounds(target);
        float radius = shot.radiusOverride > 0 ? shot.radiusOverride : Mathf.Max(b.extents.magnitude, 0.3f);
        Vector3 center = b.center;
        Vector3 fwd = target.transform.forward; fwd.y = 0; if (fwd.sqrMagnitude < 1e-4f) fwd = Vector3.forward; fwd.Normalize();
        Vector3 look = center + V(shot.lookOffset, Vector3.zero) * radius;
        cam.fieldOfView = anc != null ? ancFov : shot.fov;
        Log($"toma {shot.name}: centro {center} radio {radius:F2}");

        string dir = Path.Combine(Root, shot.name);
        Directory.CreateDirectory(dir);
        foreach (var f in Directory.GetFiles(dir, "*.jpg")) File.Delete(f);

        var pending = new List<Ev>(shot.events ?? new Ev[0]);
        int fps = Mathf.Max(list.fps, 1);
        int frames = Mathf.RoundToInt(shot.duration * fps);
        Time.captureFramerate = fps;
        var stills = new HashSet<int> { 0, frames / 2, frames - 1 };

        for (int i = 0; i < frames; i++)
        {
            float t = i / (float)fps;
            float u = Ease(t / shot.duration);
            for (int e = pending.Count - 1; e >= 0; e--)
                if (t >= pending[e].time) { Fire(pending[e], target); pending.RemoveAt(e); }

            Vector3 pos;
            if (anc != null)
            {
                Vector3 basePos = anc.position;
                Quaternion baseRot = anc.rotation;
                if (aimPoint.HasValue) baseRot = Quaternion.LookRotation(aimPoint.Value - basePos, Vector3.up);
                Vector3 f = baseRot * Vector3.forward, rgt = baseRot * Vector3.right;
                pos = basePos + f * (shot.push * u) + rgt * Mathf.Lerp(-shot.slide, shot.slide, u) * 0.5f + Vector3.up * (shot.rise * u);
                cam.transform.position = pos;
                cam.transform.rotation = aimPoint.HasValue
                    ? Quaternion.LookRotation(aimPoint.Value - pos, Vector3.up) * Quaternion.Euler(0, Mathf.Lerp(-shot.yaw, shot.yaw, u) * 0.5f, 0)
                    : baseRot * Quaternion.Euler(0, Mathf.Lerp(-shot.yaw, shot.yaw, u) * 0.5f, 0);
                goto capture;
            }
            if (shot.move == "world")
            {
                pos = Vector3.Lerp(V(shot.from, center), V(shot.to, V(shot.from, center)), u);
                cam.transform.position = pos;
                cam.transform.rotation = Quaternion.LookRotation(V(shot.lookAt, center) - pos, Vector3.up);
                goto capture;
            }
            if (shot.move == "path")
                pos = center + Vector3.Lerp(V(shot.from, Vector3.back), V(shot.to, Vector3.back), u) * radius;
            else if (shot.move == "static")
                pos = center + V(shot.from, Vector3.back) * radius;
            else
            {
                float ang = Mathf.Lerp(shot.startAngle, shot.endAngle, u);
                float dist = Mathf.Lerp(shot.startDist, shot.endDist, u) * radius;
                float h = Mathf.Lerp(shot.startHeight, shot.endHeight, u) * radius;
                Vector3 d = Quaternion.Euler(0, ang, 0) * fwd;
                pos = center + d * dist + Vector3.up * h;
            }
            cam.transform.position = pos;
            cam.transform.rotation = Quaternion.LookRotation(look - pos, Vector3.up);

            capture:
            yield return new WaitForEndOfFrame();
            if (Mode == 2 || stills.Contains(i))
            {
                var tex = ScreenCapture.CaptureScreenshotAsTexture();
                File.WriteAllBytes(Path.Combine(dir, $"f_{i:00000}.jpg"), tex.EncodeToJPG(list.jpgQuality));
                Destroy(tex);
            }
        }
    }

    static float AnchorFov(GameObject g, float def)
    {
        var c = g.GetComponent<Camera>(); if (c) return c.fieldOfView;
        foreach (var comp in g.GetComponents<Component>())
        {
            if (comp == null || comp.GetType().Name != "CinemachineCamera") continue;
            var f = comp.GetType().GetField("Lens");
            if (f == null) break;
            var lens = f.GetValue(comp);
            var fv = lens.GetType().GetField("FieldOfView");
            if (fv != null) return (float)fv.GetValue(lens);
        }
        return def;
    }

    void Fire(Ev ev, GameObject shotTarget)
    {
        try
        {
            if (!string.IsNullOrEmpty(ev.activate))
            {
                var a = FindPath(ev.activate);
                if (a) { EnsureActive(a); foreach (var ps in a.GetComponentsInChildren<ParticleSystem>(true)) ps.Play(); }
            }
            if (string.IsNullOrEmpty(ev.component)) return;
            GameObject root = string.IsNullOrEmpty(ev.target) ? shotTarget : FindPath(ev.target);
            if (root == null) { Log("evento sin objeto: " + ev.target); return; }
            var comps = new List<Component>();
            foreach (var c in root.GetComponentsInChildren<Component>(true))
                if (c != null && c.GetType().Name == ev.component) comps.Add(c);
            if (comps.Count == 0)   // buscar en toda la escena
                foreach (var c in Resources.FindObjectsOfTypeAll<Component>())
                    if (c != null && c.gameObject.scene.IsValid() && c.GetType().Name == ev.component) { comps.Add(c); break; }
            if (comps.Count == 0) { Log("componente no encontrado: " + ev.component); return; }
            if (!ev.all) comps = new List<Component> { comps[0] };
            StartCoroutine(InvokeAll(comps, ev));
        }
        catch (Exception e) { Log("error evento: " + e.Message); }
    }

    IEnumerator InvokeAll(List<Component> comps, Ev ev)
    {
        foreach (var c in comps)
        {
            Invoke(c, ev);
            if (ev.all && ev.stagger > 0) yield return new WaitForSeconds(ev.stagger);
        }
    }

    void Invoke(Component c, Ev ev)
    {
        var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        int n = ev.args != null ? ev.args.Length : 0;
        foreach (var m in c.GetType().GetMethods(flags))
        {
            if (m.Name != ev.method || m.GetParameters().Length != n) continue;
            var ps = m.GetParameters(); var vals = new object[n];
            for (int i = 0; i < n; i++)
            {
                var pt = ps[i].ParameterType;
                vals[i] = pt == typeof(int) ? (object)(int)ev.args[i] : pt == typeof(bool) ? (object)(ev.args[i] != 0) : (object)ev.args[i];
            }
            var r = m.Invoke(c, vals);
            if (r is IEnumerator it && c is MonoBehaviour mb) mb.StartCoroutine(it);
            Log($"evento {ev.component}.{ev.method} en {c.name}");
            return;
        }
        Log("metodo no encontrado: " + ev.component + "." + ev.method);
    }
}
