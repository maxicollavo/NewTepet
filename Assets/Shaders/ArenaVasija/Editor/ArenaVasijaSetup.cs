// Tepet's Pyramid - Agrega el relleno de arena a todas las vasijas de la escena (mundo y mano).
// Menu: Tepet > Vasijas > Agregar arena a las vasijas. Se puede deshacer con Ctrl+Z.
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class ArenaVasijaSetup
{
    const string ChildName = "ArenaRelleno";
    const string MatPath = "Assets/Shaders/ArenaVasija/M_ArenaVasija.mat";

    [MenuItem("Tepet/Vasijas/Agregar arena a las vasijas")]
    static void Agregar()
    {
        var mat = AssetDatabase.LoadAssetAtPath<Material>(MatPath);
        if (mat == null) { EditorUtility.DisplayDialog("Arena vasijas", "No encuentro " + MatPath, "OK"); return; }
        int hechas = 0, saltadas = 0;
        foreach (var t in Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            bool esVasija = t.GetComponent("Vase") != null || t.GetComponent("HandVases") != null;
            if (!esVasija) continue;
            if (t.Find(ChildName) != null) { saltadas++; continue; }
            Crear(t, mat);
            hechas++;
        }
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log($"[Arena vasijas] agregado a {hechas} vasijas ({saltadas} ya lo tenian). Guarda la escena para conservarlo.");
    }

    [MenuItem("Tepet/Vasijas/Quitar arena de las vasijas")]
    static void Quitar()
    {
        int n = 0;
        foreach (var t in Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            if (t != null && t.name == ChildName && t.GetComponent<VasijaArenaVisual>() != null) { Undo.DestroyObjectImmediate(t.gameObject); n++; }
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log($"[Arena vasijas] quitado de {n} vasijas.");
    }

    static void Crear(Transform vasija, Material mat)
    {
        // limites de la vasija en su propio espacio
        Bounds b = new Bounds(Vector3.zero, Vector3.zero); bool tiene = false;
        foreach (var mf in vasija.GetComponentsInChildren<MeshFilter>(true))
        {
            if (mf.sharedMesh == null || mf.name == ChildName) continue;
            var mb = mf.sharedMesh.bounds;
            for (int i = 0; i < 8; i++)
            {
                var c = mb.center + Vector3.Scale(mb.extents, new Vector3((i & 1) == 0 ? -1 : 1, (i & 2) == 0 ? -1 : 1, (i & 4) == 0 ? -1 : 1));
                var p = vasija.InverseTransformPoint(mf.transform.TransformPoint(c));
                if (!tiene) { b = new Bounds(p, Vector3.zero); tiene = true; } else b.Encapsulate(p);
            }
        }
        if (!tiene) b = new Bounds(Vector3.zero, Vector3.one);

        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        Undo.RegisterCreatedObjectUndo(go, "Arena vasija");
        go.name = ChildName;
        Object.DestroyImmediate(go.GetComponent<Collider>());   // que no tape el agarre ni la interaccion
        go.layer = vasija.gameObject.layer;
        go.transform.SetParent(vasija, false);
        // PRUEBA con los corazones: un cubo un poco mas grande que la vasija, asi la arena se ve subir por afuera.
        // Con los modelos reales: usar la forma interior de la vasija (mas chica que la vasija).
        go.transform.localPosition = b.center;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = b.size * 1.1f;
        var r = go.GetComponent<MeshRenderer>();
        r.sharedMaterial = mat;
        if (vasija.GetComponent("HandVases") != null) r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        var v = go.AddComponent<VasijaArenaVisual>();
        v.llenado = 0f;
    }

    // Pone un modelo de jarron real en cada vasija (piso y mano) en vez del cubo, y la arena ADENTRO:
    // el mismo modelo achicado, asi la arena tiene la forma del jarron y no se asoma por afuera.
    [MenuItem("Tepet/Vasijas/Poner modelos de jarrones con arena adentro")]
    static void PonerModelos() => PonerModelos(false);

    // Los que se agarran usan el jarron roto (el modelo nuevo)
    [MenuItem("Tepet/Vasijas/Usar jarron roto en los que se agarran")]
    static void PonerRoto() => PonerModelos(true);

    static void PonerModelos(bool roto)
    {
        var mat = AssetDatabase.LoadAssetAtPath<Material>(MatPath);
        var pref = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Art/Assets/EgyptAssets/Jarrones/Jarrones.prefab");
        var vasija = AssetDatabase.LoadAllAssetsAtPath("Assets/Art/Assets/EgyptAssets/Vasija con UV/vasija con uv.fbx").OfType<Mesh>().FirstOrDefault();
        var matVasija = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Assets/EgyptAssets/Jarrones/Texturas Jarrones/Jarron.mat");
        if (mat == null || pref == null || vasija == null) { EditorUtility.DisplayDialog("Jarrones", "Falta el material de arena o algun modelo.", "OK"); return; }

        // los 4 modelos: 3 de Jarrones + la vasija
        var modelos = new List<(Mesh mesh, Material[] mats)>();
        foreach (var r in pref.GetComponentsInChildren<MeshRenderer>(true))
        {
            var mf = r.GetComponent<MeshFilter>(); if (mf && mf.sharedMesh) modelos.Add((mf.sharedMesh, r.sharedMaterials));
        }
        modelos.Add((vasija, new[] { matVasija }));
        if (roto)
        {
            var mRoto = AssetDatabase.LoadAllAssetsAtPath("Assets/Art/Assets/EgyptAssets/Jarrones/jarron roto/jarron roto 2.fbx").OfType<Mesh>().FirstOrDefault();
            var matRoto = AssetDatabase.LoadAssetAtPath<Material>("Assets/Art/Assets/EgyptAssets/Jarrones/jarron roto/Jarron.mat");
            if (mRoto == null) { EditorUtility.DisplayDialog("Jarrones", "No encuentro jarron roto 2.fbx", "OK"); return; }
            modelos.Clear(); modelos.Add((mRoto, new[] { matRoto }));
        }

        int n = 0;
        var lista = Object.FindObjectsByType<Vase>(FindObjectsInactive.Include, FindObjectsSortMode.None).Select(v => v.transform)
            .Concat(Object.FindObjectsByType<HandVases>(FindObjectsInactive.Include, FindObjectsSortMode.None).Select(v => v.transform)).ToList();
        foreach (var t in lista)
        {
            if (t == null) continue;
            var vase = t.GetComponent<Vase>(); var hand = t.GetComponent<HandVases>();
            int idx = vase ? vase.vaseIndex : hand.vaseIndex;
            var (mesh, mats) = modelos[Mathf.Abs(idx) % modelos.Count];

            // el cubo deja de verse, pero sigue estando (collider y scripts no se tocan)
            var cubo = t.GetComponent<MeshRenderer>(); var cuboMf = t.GetComponent<MeshFilter>();
            Bounds caja = cuboMf && cuboMf.sharedMesh ? cuboMf.sharedMesh.bounds : new Bounds(Vector3.zero, Vector3.one);
            if (cubo) { Undo.RecordObject(cubo, "Jarrones"); cubo.enabled = false; }

            foreach (var n2 in new[] { "Modelo", ChildName }) { var v = t.Find(n2); if (v) Undo.DestroyObjectImmediate(v.gameObject); }

            // modelo: entra en la caja del cubo, apoyado abajo
            var mod = new GameObject("Modelo", typeof(MeshFilter), typeof(MeshRenderer));
            Undo.RegisterCreatedObjectUndo(mod, "Jarrones");
            mod.layer = t.gameObject.layer;
            mod.transform.SetParent(t, false);
            mod.GetComponent<MeshFilter>().sharedMesh = mesh;
            var mr = mod.GetComponent<MeshRenderer>(); mr.sharedMaterials = mats;
            if (hand) mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            var mb = mesh.bounds;
            float k = Mathf.Min(caja.size.x / mb.size.x, caja.size.y / mb.size.y, caja.size.z / mb.size.z);
            mod.transform.localScale = Vector3.one * k;
            mod.transform.localPosition = new Vector3(caja.center.x - mb.center.x * k, caja.min.y - mb.min.y * k, caja.center.z - mb.center.z * k);
            if (roto) mod.transform.RotateAround(t.TransformPoint(caja.center), t.up, idx * 97f);

            // arena: un cilindro cerrado del ancho del cuello, desde el fondo hasta la boca.
            // (el modelo es hueco, asi que no sirve como relleno: se veria el agujero)
            var verts = mesh.vertices;
            float cx = mb.center.x, cz = mb.center.z, top = mb.max.y, fondo = mb.min.y + mb.size.y * 0.08f;
            float rCuello = float.MaxValue;
            foreach (var v in verts)
                if (v.y > top - mb.size.y * 0.3f) rCuello = Mathf.Min(rCuello, new Vector2(v.x - cx, v.z - cz).magnitude);
            if (rCuello == float.MaxValue || rCuello < mb.size.x * 0.05f) rCuello = mb.size.x * 0.25f;
            rCuello *= 0.95f;
            var arena = GameObject.CreatePrimitive(PrimitiveType.Cylinder); arena.name = ChildName;
            Object.DestroyImmediate(arena.GetComponent<Collider>());
            Undo.RegisterCreatedObjectUndo(arena, "Jarrones");
            arena.layer = t.gameObject.layer;
            arena.transform.SetParent(mod.transform, false);
            var ar = arena.GetComponent<MeshRenderer>(); ar.sharedMaterial = mat;
            ar.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            float tope = top - mb.size.y * 0.08f;   // un poco abajo de la boca: lleno no se ven agujeros
            arena.transform.localPosition = new Vector3(cx, (fondo + tope) / 2f, cz);
            arena.transform.localScale = new Vector3(rCuello * 2f, (tope - fondo) / 2f, rCuello * 2f);
            var vis = arena.AddComponent<VasijaArenaVisual>(); vis.llenado = 0f;
            n++;
        }
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log($"[Jarrones] {n} jarrones con modelo y arena adentro ({(roto ? "jarron roto" : modelos.Count + " modelos distintos")}).");
    }

    // Las placas de presion aceptan un jarron: click con el jarron en la mano y queda apoyado.
    [MenuItem("Tepet/Vasijas/Preparar placas para apoyar jarrones")]
    static void PrepararPlacas()
    {
        int n = 0;
        foreach (var r in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (!r.name.StartsWith("Pressure_Plate")) continue;
            var go = r;
            if (go.GetComponent<Collider>() == null)
            {
                var rs = go.GetComponentsInChildren<Renderer>(true);
                if (rs.Length == 0) continue;
                Bounds b = rs[0].bounds; foreach (var x in rs) b.Encapsulate(x.bounds);
                var bc = Undo.AddComponent<BoxCollider>(go);
                bc.center = go.transform.InverseTransformPoint(b.center);
                var s = go.transform.lossyScale;
                bc.size = new Vector3(b.size.x / Mathf.Abs(s.x), b.size.y / Mathf.Abs(s.y), b.size.z / Mathf.Abs(s.z));
            }
            if (go.GetComponent<Outline>() == null) Undo.AddComponent<Outline>(go);
            if (go.GetComponent<ApoyarJarron>() == null) Undo.AddComponent<ApoyarJarron>(go);
            n++;
        }
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log($"[Jarrones] {n} placas listas para apoyar jarrones.");
    }

    // Los jarrones que se agarran pasan a ser los cuencos de arena que ya estan en la sala (Cuenco_Arena_...).
    // El del piso queda exactamente donde estaba el cuenco; el de la mano usa el mismo modelo.
    [MenuItem("Tepet/Vasijas/Usar los cuencos de arena de la sala")]
    static void UsarCuencos()
    {
        var mat = AssetDatabase.LoadAssetAtPath<Material>(MatPath);
        var cuencos = Object.FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include, FindObjectsSortMode.None)
            .Where(r => r.name.StartsWith("Cuenco_Arena") && r.GetComponentInParent<Vase>(true) == null && r.GetComponentInParent<HandVases>(true) == null)
            .OrderBy(r => r.name).ToList();
        if (mat == null || cuencos.Count == 0) { EditorUtility.DisplayDialog("Jarrones", "No encuentro los Cuenco_Arena en la escena.", "OK"); return; }

        int n = 0;
        foreach (var vase in Object.FindObjectsByType<Vase>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            var c = cuencos[Mathf.Abs(vase.vaseIndex) % cuencos.Count];
            var mesh = c.GetComponent<MeshFilter>().sharedMesh;
            var t = vase.transform;
            Undo.RecordObject(t, "Cuencos");
            t.position = c.transform.position;
            var mod = NuevoModelo(t, mesh, c.sharedMaterials, false);
            mod.transform.SetPositionAndRotation(c.transform.position, c.transform.rotation);
            var ls = t.lossyScale; var cs = c.transform.lossyScale;
            mod.transform.localScale = new Vector3(cs.x / ls.x, cs.y / ls.y, cs.z / ls.z);
            // el collider del agarre del tamaño del cuenco
            var bc = t.GetComponent<BoxCollider>();
            if (bc)
            {
                Undo.RecordObject(bc, "Cuencos");
                var mb = mesh.bounds;
                bc.center = t.InverseTransformPoint(mod.transform.TransformPoint(mb.center));
                bc.size = Vector3.Scale(mb.size, mod.transform.localScale);
            }
            ArenaAdentro(mod, mesh, mat);
            Undo.RecordObject(c.gameObject, "Cuencos");
            c.gameObject.SetActive(false);   // queda el original apagado, no se borra
            n++;
        }
        foreach (var hand in Object.FindObjectsByType<HandVases>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            var c = cuencos[Mathf.Abs(hand.vaseIndex) % cuencos.Count];
            var mesh = c.GetComponent<MeshFilter>().sharedMesh;
            var t = hand.transform;
            var cuboMf = t.GetComponent<MeshFilter>();
            Bounds caja = cuboMf && cuboMf.sharedMesh ? cuboMf.sharedMesh.bounds : new Bounds(Vector3.zero, Vector3.one);
            var mod = NuevoModelo(t, mesh, c.sharedMaterials, true);
            var mb = mesh.bounds;
            float k = Mathf.Min(caja.size.x / mb.size.x, caja.size.y / mb.size.y, caja.size.z / mb.size.z);
            mod.transform.localScale = Vector3.one * k;
            mod.transform.localPosition = new Vector3(caja.center.x - mb.center.x * k, caja.min.y - mb.min.y * k, caja.center.z - mb.center.z * k);
            MirarACamara(mod, mesh);
            ArenaAdentro(mod, mesh, mat);
            n++;
        }
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log($"[Jarrones] {n} jarrones ahora son los cuencos de arena ({cuencos.Count} cuencos, los originales quedaron apagados).");
    }

    static GameObject NuevoModelo(Transform t, Mesh mesh, Material[] mats, bool mano)
    {
        var cubo = t.GetComponent<MeshRenderer>();
        if (cubo) { Undo.RecordObject(cubo, "Cuencos"); cubo.enabled = false; }
        foreach (var n2 in new[] { "Modelo", ChildName }) { var v = t.Find(n2); if (v) Undo.DestroyObjectImmediate(v.gameObject); }
        var mod = new GameObject("Modelo", typeof(MeshFilter), typeof(MeshRenderer));
        Undo.RegisterCreatedObjectUndo(mod, "Cuencos");
        mod.layer = t.gameObject.layer;
        mod.transform.SetParent(t, false);
        mod.GetComponent<MeshFilter>().sharedMesh = mesh;
        var mr = mod.GetComponent<MeshRenderer>(); mr.sharedMaterials = mats;
        if (mano) mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        return mod;
    }

    // la arena es una copia del mismo cuenco, mas chica y adentro del original.
    // se llena solo hasta el borde mas bajo (en los rotos, hasta donde esta roto) para que no se salga.
    const float EscalaArenaXZ = 0.86f, EscalaArenaY = 0.88f;

    static void ArenaAdentro(GameObject mod, Mesh mesh, Material mat)
    {
        var mb = mesh.bounds;
        var arena = new GameObject(ChildName, typeof(MeshFilter), typeof(MeshRenderer));
        Undo.RegisterCreatedObjectUndo(arena, "Cuencos");
        arena.layer = mod.layer;
        arena.transform.SetParent(mod.transform, false);
        arena.GetComponent<MeshFilter>().sharedMesh = mesh;
        var ar = arena.GetComponent<MeshRenderer>(); ar.sharedMaterial = mat;
        ar.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        var s = new Vector3(EscalaArenaXZ, EscalaArenaY, EscalaArenaXZ);
        arena.transform.localScale = s;
        arena.transform.localPosition = new Vector3(mb.center.x * (1 - s.x), mb.min.y * (1 - s.y) + mb.size.y * 0.1f, mb.center.z * (1 - s.z));

        float bajo = BordeMasBajo(mesh, out _);
        var vis = arena.AddComponent<VasijaArenaVisual>(); vis.llenado = 0f;
        // tope en el espacio de la copia: el borde bajo del original menos un margen
        float topeOriginal = bajo - mb.size.y * 0.06f;
        float yCopiaMin = mb.min.y * s.y + arena.transform.localPosition.y, yCopiaMax = mb.max.y * s.y + arena.transform.localPosition.y;
        vis.tope = Mathf.Clamp((topeOriginal - yCopiaMin) / (yCopiaMax - yCopiaMin), 0.15f, 1f);
    }

    // altura del punto mas bajo del borde (el lado roto) y hacia donde queda, en espacio del mesh
    static float BordeMasBajo(Mesh mesh, out Vector3 direccion)
    {
        var mb = mesh.bounds; const int N = 24;
        var alto = new float[N]; for (int k = 0; k < N; k++) alto[k] = float.MinValue;
        float rMax = 0f; var verts = mesh.vertices;
        foreach (var v in verts) rMax = Mathf.Max(rMax, new Vector2(v.x - mb.center.x, v.z - mb.center.z).magnitude);
        foreach (var v in verts)
        {
            var d = new Vector2(v.x - mb.center.x, v.z - mb.center.z);
            if (d.magnitude < rMax * 0.6f) continue;
            int k = Mathf.FloorToInt((Mathf.Atan2(d.y, d.x) + Mathf.PI) / (2 * Mathf.PI) * N) % N;
            alto[k] = Mathf.Max(alto[k], v.y);
        }
        float bajo = mb.max.y; int kb = 0;
        for (int k = 0; k < N; k++) if (alto[k] > float.MinValue && alto[k] < bajo) { bajo = alto[k]; kb = k; }
        float ang = (kb + 0.5f) / N * 2 * Mathf.PI - Mathf.PI;
        direccion = new Vector3(Mathf.Cos(ang), 0f, Mathf.Sin(ang));
        return bajo;
    }

    // el cuenco de la mano gira para que el lado roto (por donde se ve la arena) quede mirando a la camara
    static void MirarACamara(GameObject mod, Mesh mesh)
    {
        var cam = mod.GetComponentInParent<Camera>(true);
        if (cam == null) cam = Camera.main;
        if (cam == null) return;
        BordeMasBajo(mesh, out var dirMesh);
        var mb = mesh.bounds;
        Vector3 centro = mod.transform.TransformPoint(mb.center);
        Vector3 dirMundo = mod.transform.TransformDirection(dirMesh);
        Vector3 aCam = cam.transform.position - centro;
        Vector3 up = mod.transform.up;
        dirMundo = Vector3.ProjectOnPlane(dirMundo, up); aCam = Vector3.ProjectOnPlane(aCam, up);
        if (dirMundo.sqrMagnitude < 1e-6f || aCam.sqrMagnitude < 1e-6f) return;
        float ang = Vector3.SignedAngle(dirMundo, aCam, up);
        mod.transform.RotateAround(centro, up, ang);
    }

    // El cubo gris de la cascada va justo debajo del chorro, invisible y sin bloquear al jugador.
    [MenuItem("Tepet/Vasijas/Poner la cascada en la caida")]
    static void CascadaEnLaCaida()
    {
        var sc = GameObject.Find("SandCascade"); var ca = GameObject.Find("CascadaArena");
        if (!sc || !ca) { EditorUtility.DisplayDialog("Cascada", "No encuentro SandCascade o CascadaArena.", "OK"); return; }
        Undo.RecordObject(sc.transform, "Cascada");
        float piso = sc.transform.position.y - sc.transform.lossyScale.y * 0.5f;
        if (Physics.Raycast(new Vector3(ca.transform.position.x, ca.transform.position.y - 1f, ca.transform.position.z), Vector3.down, out var hit, 60f)) piso = hit.point.y;
        float alto = Mathf.Max(1f, ca.transform.position.y - piso);
        var p = sc.transform.parent;
        sc.transform.position = new Vector3(ca.transform.position.x, piso + alto * 0.5f, ca.transform.position.z);
        var esc = new Vector3(1.2f, alto, 1.2f);
        if (p) esc = new Vector3(esc.x / p.lossyScale.x, esc.y / p.lossyScale.y, esc.z / p.lossyScale.z);
        sc.transform.localScale = esc;
        var r = sc.GetComponent<MeshRenderer>(); if (r) { Undo.RecordObject(r, "Cascada"); r.enabled = false; }
        var c = sc.GetComponent<Collider>(); if (c) { Undo.RecordObject(c, "Cascada"); c.isTrigger = true; }
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log($"[Cascada] SandCascade debajo del chorro (piso {piso:F2}, alto {alto:F1} m), invisible y sin bloquear.");
    }
}
