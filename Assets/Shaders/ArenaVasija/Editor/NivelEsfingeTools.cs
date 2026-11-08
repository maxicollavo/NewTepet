// Tepet's Pyramid - herramientas para armar la sala de la esfinge (solo editor).
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public static class NivelEsfingeTools
{
    static string Out => Path.GetFullPath(Path.Combine(Application.dataPath, "..", "TrailerCaptures", "medidas.txt"));

    [MenuItem("Tepet/Nivel Esfinge/Medir paredes y piso")]
    static void Medir()
    {
        var sb = new StringBuilder();
        foreach (var t in Object.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None))
        {
            string n = t.name;
            bool interesa = n.StartsWith("Pared Ladrillos") || n == "Sala SUBTERRANEA" || n == "Pisos" || n == "TuboAscensor"
                            || n == "Pared Lisa Tunel" || n == "Esfinge" || n.StartsWith("Piso Cueva") || n.StartsWith("plano")
                            || n.StartsWith("Pressure Plate") || n == "SandCascade" || n == "CascadaArena" || n.StartsWith("Fan");
            if (!interesa) continue;
            var rs = t.GetComponentsInChildren<Renderer>(true);
            if (rs.Length == 0) { sb.AppendLine($"{n}\tpos {V(t.position)}\tsin renderer"); continue; }
            Bounds b = rs[0].bounds; foreach (var r in rs) b.Encapsulate(r.bounds);
            var mf = t.GetComponentInChildren<MeshFilter>(true);
            string local = mf && mf.sharedMesh ? $"\tmeshLocal min {V(mf.sharedMesh.bounds.min)} max {V(mf.sharedMesh.bounds.max)}" : "";
            sb.AppendLine($"{n}\tactivo {t.gameObject.activeInHierarchy}\tpos {V(t.position)}\trotY {t.eulerAngles.y:F1}\tescala {V(t.lossyScale)}\tbounds min {V(b.min)} max {V(b.max)}{local}");
        }
        File.WriteAllText(Out, sb.ToString());
        Debug.Log("[Nivel Esfinge] medidas en " + Out);
    }


    // ---------------- armado de paredes ----------------
    const string Grupo = "Paredes Sala Esfinge";
    const string GrupoPiso = "Piso extra Sala Esfinge";
    const float EscalaBase = 0.012016378f;   // la de las Pared Ladrillos ya puestas
    const float Largo = 400f * EscalaBase;   // 4.8066 m (mesh: 400 en Z local, 381.5 de alto)
    const float YPared = -34.65f;
    const float YPiso = -34.287f;
    const int PanelesPorLado = 3;            // paneles a cada lado del tunel (ancho de la sala)
    const int Filas = 4;                     // filas de pared en altura: 4 x 4.58 m = 18.3 m (la esfinge mide 8.7 y los abanicos 17.5)

    // segmento de pared de A a B (en XZ). La cara visible mira hacia el interior: el lado izquierdo yendo de A a B.
    static void Segmento(GameObject prefab, Transform padre, Vector2 a, Vector2 b, string nombre, Material[] mats, int filaDesde = 0, int filaHasta = Filas)
    {
        Vector2 d = b - a; float len = d.magnitude; int n = Mathf.Max(1, Mathf.RoundToInt(len / Largo));
        float paso = len / n, factor = paso / Largo;
        // la pared corre por su Z local; rotY tal que Z local apunte de A a B
        float rotY = Mathf.Atan2(d.x, d.y) * Mathf.Rad2Deg;
        for (int fila = filaDesde; fila < filaHasta; fila++)
        for (int i = 0; i < n; i++)
        {
            Vector2 p = a + d.normalized * (paso * i);
            var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab, padre);
            Undo.RegisterCreatedObjectUndo(go, "Paredes sala esfinge");
            go.name = $"{nombre} {i + 1} fila {fila + 1}";
            go.transform.position = new Vector3(p.x, YPared + fila * 381.5f * EscalaBase, p.y);
            go.transform.rotation = Quaternion.Euler(0, rotY, 0);
            go.transform.localScale = new Vector3(EscalaBase, EscalaBase, EscalaBase * factor);
            if (mats != null) foreach (var r in go.GetComponentsInChildren<Renderer>(true)) r.sharedMaterials = mats;
        }
    }

    [MenuItem("Tepet/Nivel Esfinge/Armar paredes de la sala")]
    static void ArmarParedes()
    {
        var refPared = GameObject.Find("Pared Ladrillos (1)");
        var sala = GameObject.Find("Sala SUBTERRANEA");
        if (refPared == null || sala == null) { EditorUtility.DisplayDialog("Nivel Esfinge", "No encuentro 'Pared Ladrillos (1)' o 'Sala SUBTERRANEA'.", "OK"); return; }
        var prefab = PrefabUtility.GetCorrespondingObjectFromSource(refPared);
        if (prefab == null) { EditorUtility.DisplayDialog("Nivel Esfinge", "'Pared Ladrillos (1)' no es un prefab.", "OK"); return; }
        if (sala.transform.Find(Grupo) != null) { EditorUtility.DisplayDialog("Nivel Esfinge", "Ya existen las paredes. Usá 'Quitar paredes de la sala' primero.", "OK"); return; }

        var refRend = refPared.GetComponentInChildren<Renderer>(true);
        var mats = refRend ? refRend.sharedMaterials : null;   // el material que tiene la pared del tunel (Ladrillos)
        var grupo = new GameObject(Grupo); Undo.RegisterCreatedObjectUndo(grupo, "Paredes sala esfinge");
        grupo.transform.SetParent(sala.transform, true);

        // esquinas tomadas de las paredes del tunel ya puestas (x 178.514 y 183.348, boca en z -112.175)
        float xTunO = 178.514f, xTunE = 183.348f, zNorte = -112.175f;
        float xO = xTunO - PanelesPorLado * Largo;   // pared oeste
        float xE = xTunE + PanelesPorLado * Largo;   // pared este
        float zSur = zNorte - 6 * Largo;            // pared sur
        float xNichoO = xTunO - Largo, xNichoE = xTunE;   // nicho de la esfinge (2 paneles de ancho)
        float zNicho = zSur - Largo;                // fondo del nicho (1 panel de profundidad)

        Vector2 V2(float x, float z) => new Vector2(x, z);
        // recorrido en sentido horario visto desde arriba: el interior queda a la derecha... se ajusta rotando
        Segmento(prefab, grupo.transform, V2(xO, zNorte), V2(xTunO, zNorte), "Norte O", mats);
        Segmento(prefab, grupo.transform, V2(xTunO, zNorte), V2(xTunE, zNorte), "Norte sobre tunel", mats, 1, Filas);
        Segmento(prefab, grupo.transform, V2(xTunE, zNorte), V2(xE, zNorte), "Norte E", mats);
        Segmento(prefab, grupo.transform, V2(xE, zNorte), V2(xE, zSur), "Este", mats);
        Segmento(prefab, grupo.transform, V2(xE, zSur), V2(xNichoE, zSur), "Sur E", mats);
        Segmento(prefab, grupo.transform, V2(xNichoE, zSur), V2(xNichoE, zNicho), "Nicho E", mats);
        Segmento(prefab, grupo.transform, V2(xNichoE, zNicho), V2(xNichoO, zNicho), "Nicho fondo", mats);
        Segmento(prefab, grupo.transform, V2(xNichoO, zNicho), V2(xNichoO, zSur), "Nicho O", mats);
        Segmento(prefab, grupo.transform, V2(xNichoO, zSur), V2(xO, zSur), "Sur O", mats);
        Segmento(prefab, grupo.transform, V2(xO, zSur), V2(xO, zNorte), "Oeste", mats);

        // piso extra: franja oeste y piso del nicho (copias de Piso Cueva)
        var refPiso = GameObject.Find("Piso Cueva");
        var prefabPiso = refPiso ? PrefabUtility.GetCorrespondingObjectFromSource(refPiso) : null;
        if (prefabPiso != null && sala.transform.Find(GrupoPiso) == null)
        {
            var gp = new GameObject(GrupoPiso); Undo.RegisterCreatedObjectUndo(gp, "Paredes sala esfinge");
            gp.transform.SetParent(sala.transform, true);
            var escalaPiso = refPiso.transform.lossyScale;
            Vector3[] centros = {
                new Vector3(165.615f, YPiso, -113.78f), new Vector3(165.615f, YPiso, -121.78f),
                new Vector3(165.615f, YPiso, -129.78f), new Vector3(165.615f, YPiso, -137.78f),
                new Vector3(177.615f, YPiso, -145.78f), new Vector3(181.615f, YPiso, -145.78f) };
            int k = 0;
            foreach (var c in centros)
            {
                var go = (GameObject)PrefabUtility.InstantiatePrefab(prefabPiso, gp.transform);
                Undo.RegisterCreatedObjectUndo(go, "Paredes sala esfinge");
                go.name = $"Piso Cueva extra {++k}";
                go.transform.position = c; go.transform.rotation = refPiso.transform.rotation;
                go.transform.localScale = escalaPiso;
                CopiarMateriales(refPiso, go);
            }
        }
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(sala.scene);
        Selection.activeGameObject = grupo;
        Debug.Log($"[Nivel Esfinge] paredes armadas: oeste x {xO:F3}, este x {xE:F3}, norte z {zNorte:F3}, sur z {zSur:F3}, nicho hasta z {zNicho:F3}");
    }

    [MenuItem("Tepet/Nivel Esfinge/Quitar paredes de la sala")]
    static void QuitarParedes()
    {
        var sala = GameObject.Find("Sala SUBTERRANEA");
        if (sala == null) return;
        foreach (var n in new[] { Grupo })   // el piso extra se deja (puede tener ajustes)
        {
            var t = sala.transform.Find(n);
            if (t) Undo.DestroyObjectImmediate(t.gameObject);
        }
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(sala.scene);
    }


    static void CopiarMateriales(GameObject desde, GameObject hacia)
    {
        var a = desde.GetComponentsInChildren<Renderer>(true);
        var b = hacia.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < Mathf.Min(a.Length, b.Length); i++)
        {
            Undo.RecordObject(b[i], "Material piso");
            b[i].sharedMaterials = a[i].sharedMaterials;
        }
    }

    [MenuItem("Tepet/Nivel Esfinge/Poner material del piso a los extras")]
    static void MaterialPisoExtras()
    {
        var refPiso = GameObject.Find("Piso Cueva");
        var sala = GameObject.Find("Sala SUBTERRANEA");
        var gp = sala ? sala.transform.Find(GrupoPiso) : null;
        if (refPiso == null || gp == null) { Debug.LogWarning("[Nivel Esfinge] no encuentro 'Piso Cueva' o el grupo de piso extra"); return; }
        foreach (Transform t in gp) CopiarMateriales(refPiso, t.gameObject);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(sala.scene);
        Debug.Log("[Nivel Esfinge] material del piso copiado a " + gp.childCount + " baldosas extra");
    }


    [MenuItem("Tepet/Nivel Esfinge/Variar baldosas del piso (cada una distinta)")]
    static void VariarPiso()
    {
        var sala = GameObject.Find("Sala SUBTERRANEA");
        if (sala == null) return;
        int n = 0;
        var rnd = new System.Random(1234);
        foreach (var r in sala.GetComponentsInChildren<Renderer>(true))
        {
            if (!r.name.StartsWith("Piso Cueva") && !(r.transform.parent && r.transform.parent.name.StartsWith("Piso Cueva"))) continue;
            var v = r.GetComponent<VariacionPiso>();
            if (v == null) v = Undo.AddComponent<VariacionPiso>(r.gameObject);
            else Undo.RecordObject(v, "Variar piso");
            float f() => (float)rnd.NextDouble();
            v.relieve = 0.02f + f() * 0.03f;                 // entre 0.02 y 0.05
            v.normal = 1.2f + f() * 0.8f;                    // entre 1.2 y 2.0
            float t = 0.94f + f() * 0.08f;                   // tono un poco mas claro u oscuro
            v.tinte = new Color(t, t * (0.98f + f() * 0.03f), t * (0.95f + f() * 0.05f), 1f);
            v.desfase = new Vector2(f(), f());
            v.tiling = Vector2.one;
            v.Aplicar();
            n++;
        }
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(sala.scene);
        Debug.Log($"[Nivel Esfinge] variacion aplicada a {n} baldosas. Cada una se ajusta en su componente 'Variacion Piso'.");
    }


    // Relieve pensado por zonas: la arena se junta contra las paredes, en las esquinas y en el nicho de la esfinge;
    // el camino del tunel a las placas queda mas pisado (menos relieve, un poco mas claro).
    [MenuItem("Tepet/Nivel Esfinge/Distribuir relieve por zonas de la sala")]
    static void DistribuirRelieve()
    {
        var sala = GameObject.Find("Sala SUBTERRANEA");
        if (sala == null) return;
        float xTunO = 178.514f, xTunE = 183.348f, zNorte = -112.175f;
        float xO = xTunO - PanelesPorLado * Largo, xE = xTunE + PanelesPorLado * Largo, zSur = zNorte - 6 * Largo;
        float xCamino = (xTunO + xTunE) * 0.5f;
        var rnd = new System.Random(77);
        float f() => (float)rnd.NextDouble();
        int n = 0;
        foreach (var r in sala.GetComponentsInChildren<Renderer>(true))
        {
            if (!r.name.StartsWith("Piso Cueva")) continue;
            var v = r.GetComponent<VariacionPiso>();
            if (v == null) v = Undo.AddComponent<VariacionPiso>(r.gameObject);
            Undo.RecordObject(v, "Relieve por zonas");
            Vector3 c = r.bounds.center;
            // 0 en el centro de la sala, 1 contra una pared
            float dPared = Mathf.Min(Mathf.Min(c.x - xO, xE - c.x), Mathf.Min(zNorte - c.z, c.z - zSur));
            float cercaPared = 1f - Mathf.Clamp01(dPared / 8f);
            bool esquina = Mathf.Min(c.x - xO, xE - c.x) < 6f && Mathf.Min(zNorte - c.z, c.z - zSur) < 6f;
            bool nicho = c.z < zSur;
            // 1 sobre el camino tunel -> placas
            float camino = (c.z > -128f) ? 1f - Mathf.Clamp01(Mathf.Abs(c.x - xCamino) / 7f) : 0f;

            float monton = Mathf.Clamp01(cercaPared + (esquina ? 0.35f : 0f) + (nicho ? 0.6f : 0f) - camino * 0.7f);
            v.relieve = Mathf.Lerp(0.012f, 0.065f, monton) + (f() - 0.5f) * 0.008f;
            v.normal = Mathf.Lerp(0.9f, 2.4f, monton) + (f() - 0.5f) * 0.2f;
            float tono = Mathf.Lerp(1.06f, 0.88f, monton) + (f() - 0.5f) * 0.03f;   // pisado = mas claro, montones = sombra
            v.tinte = new Color(tono, tono * 0.99f, tono * 0.96f, 1f);
            v.desfase = new Vector2(f(), f());
            v.tiling = Vector2.one * Mathf.Lerp(1f, 1.25f, monton);                  // grano un poco mas cerrado en los montones
            v.Aplicar();
            n++;
        }
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(sala.scene);
        Debug.Log($"[Nivel Esfinge] relieve por zonas en {n} baldosas (paredes, esquinas y nicho con mas arena; camino central mas pisado).");
    }


    [MenuItem("Tepet/Nivel Esfinge/Piso de arena continuo (sin cortes)")]
    static void PisoContinuo()
    {
        var sala = GameObject.Find("Sala SUBTERRANEA");
        var mat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Shaders/ArenaVasija/M_PisoArena.mat");
        if (sala == null || mat == null) { Debug.LogWarning("[Nivel Esfinge] falta la sala o M_PisoArena"); return; }
        int n = 0;
        foreach (var r in sala.GetComponentsInChildren<Renderer>(true))
        {
            if (!r.name.StartsWith("Piso Cueva")) continue;
            var v = r.GetComponent<VariacionPiso>();
            if (v != null) Undo.DestroyObjectImmediate(v);
            r.SetPropertyBlock(null);
            Undo.RecordObject(r, "Piso continuo");
            r.sharedMaterial = mat;
            n++;
        }
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(sala.scene);
        Debug.Log($"[Nivel Esfinge] M_PisoArena en {n} baldosas. Los valores por zona se ajustan en el material.");
    }


    // Duplica el plano de arena seleccionado (o "plano (6)") 3 veces en la esquina sur-oeste, al lado del nicho de la esfinge.
    [MenuItem("Tepet/Nivel Esfinge/Arena en la esquina de la esfinge (3 copias)")]
    static void ArenaEsquina()
    {
        var src = Selection.activeGameObject;
        if (src == null || src.GetComponent<Renderer>() == null) src = GameObject.Find("plano (6)");
        if (src == null) { EditorUtility.DisplayDialog("Nivel Esfinge", "Selecciona el plano con 'Prueba de sand mat' y volve a ejecutar.", "OK"); return; }
        float xTunO = 178.514f, zNorte = -112.175f;
        float xO = xTunO - PanelesPorLado * Largo, zSur = zNorte - 6 * Largo, xNichoO = xTunO - Largo;
        Vector3 b = src.transform.lossyScale;
        float y = src.transform.position.y;
        // (x, z, escala relativa, giro): tres montones que se apoyan contra la pared sur y el costado del nicho
        var montones = new (float x, float z, float k, float rot)[] {
            (xNichoO - 2.6f, zSur + 2.4f, 0.75f, 12f),
            (xNichoO - 6.2f, zSur + 3.1f, 0.60f, -24f),
            (xO + 3.0f,      zSur + 5.6f, 0.55f, 38f) };
        var parent = src.transform.parent;
        int i = 0;
        foreach (var m in montones)
        {
            var go = Object.Instantiate(src, parent);
            Undo.RegisterCreatedObjectUndo(go, "Arena esquina esfinge");
            go.name = $"Arena esquina esfinge {++i}";
            go.transform.position = new Vector3(m.x, y + 0.01f * i, m.z);   // un poquito de altura distinta para que no parpadeen
            go.transform.rotation = Quaternion.Euler(0, m.rot, 0) * src.transform.rotation;
            var ls = src.transform.localScale;
            go.transform.localScale = new Vector3(ls.x * m.k, ls.y * (0.8f + 0.4f * m.k), ls.z * m.k);
        }
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(src.scene);
        Debug.Log($"[Nivel Esfinge] 3 copias de '{src.name}' en la esquina sur-oeste junto a la esfinge (x {xO:F1}..{xNichoO:F1}, z {zSur:F1}).");
    }


    const string GrupoTecho = "Techo Sala Esfinge";
    const float AltoPared = 381.5f * EscalaBase;   // 4.584 m

    const float LadoPozo = 3f;   // ancho del hueco/pozo de la cascada (m)

    // divide [a,b] en paneles de ~'paso'; si hay hueco, deja una celda exacta [h0,h1] y reparte el resto a los costados
    static System.Collections.Generic.List<Vector2> Rangos(float a, float b, float paso, float? h0, float? h1)
    {
        var r = new System.Collections.Generic.List<Vector2>();
        void Partir(float x0, float x1)
        {
            if (x1 - x0 < 0.05f) return;
            int n = Mathf.Max(1, Mathf.RoundToInt((x1 - x0) / paso)); float w = (x1 - x0) / n;
            for (int i = 0; i < n; i++) r.Add(new Vector2(x0 + w * i, x0 + w * (i + 1)));
        }
        if (h0.HasValue) { Partir(a, h0.Value); r.Add(new Vector2(h0.Value, h1.Value)); Partir(h1.Value, b); }
        else Partir(a, b);
        return r;
    }

    // Techo con Pared Ladrillos acostadas mirando hacia abajo. Si hay hueco, queda un hueco de LadoPozo x LadoPozo centrado en el.
    static void TechoRect(GameObject prefab, Transform padre, Material[] mats, float x0, float x1, float z0, float z1, string nombre, Vector2? hueco)
    {
        float y = YPared + AltoPared * Filas;
        float? hx0 = null, hx1 = null, hz0 = null, hz1 = null;
        if (hueco.HasValue)
        {
            hx0 = Mathf.Clamp(hueco.Value.x - LadoPozo / 2, x0 + 0.5f, x1 - LadoPozo - 0.5f); hx1 = hx0 + LadoPozo;
            hz0 = Mathf.Clamp(hueco.Value.y - LadoPozo / 2, z0 + 0.5f, z1 - LadoPozo - 0.5f); hz1 = hz0 + LadoPozo;
        }
        var rx = Rangos(x0, x1, AltoPared, hx0, hx1);
        var rz = Rangos(z0, z1, Largo, hz0, hz1);
        for (int i = 0; i < rx.Count; i++)
            for (int j = 0; j < rz.Count; j++)
            {
                Vector2 cx = rx[i], cz = rz[j];
                if (hueco.HasValue && Mathf.Approximately(cx.x, hx0.Value) && Mathf.Approximately(cz.x, hz0.Value)) continue;
                float sx = cx.y - cx.x, sz = cz.y - cz.x;
                var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab, padre);
                Undo.RegisterCreatedObjectUndo(go, "Techo sala esfinge");
                go.name = $"{nombre} {i + 1}-{j + 1}";
                go.transform.position = new Vector3(cx.x, y, cz.x);
                go.transform.rotation = Quaternion.Euler(0, 0, -90);   // cara visible hacia abajo; el alto del panel corre en X, el largo en Z
                go.transform.localScale = new Vector3(EscalaBase, EscalaBase * (sx / AltoPared), EscalaBase * (sz / Largo));
                if (mats != null) foreach (var r in go.GetComponentsInChildren<Renderer>(true)) r.sharedMaterials = mats;
            }
    }

    [MenuItem("Tepet/Nivel Esfinge/Armar techo de la sala")]
    static void ArmarTecho()
    {
        var refPared = GameObject.Find("Pared Ladrillos (1)");
        var sala = GameObject.Find("Sala SUBTERRANEA");
        if (refPared == null || sala == null) return;
        if (sala.transform.Find(GrupoTecho) != null) { EditorUtility.DisplayDialog("Nivel Esfinge", "Ya hay techo. Usa 'Quitar techo' primero.", "OK"); return; }
        var prefab = PrefabUtility.GetCorrespondingObjectFromSource(refPared);
        var r0 = refPared.GetComponentInChildren<Renderer>(true);
        var mats = r0 ? r0.sharedMaterials : null;
        var grupo = new GameObject(GrupoTecho); Undo.RegisterCreatedObjectUndo(grupo, "Techo sala esfinge");
        grupo.transform.SetParent(sala.transform, true);

        float xTunO = 178.514f, xTunE = 183.348f, zNorte = -112.175f;
        float xO = xTunO - PanelesPorLado * Largo, xE = xTunE + PanelesPorLado * Largo, zSur = zNorte - 6 * Largo;
        float xNichoO = xTunO - Largo, xNichoE = xTunE, zNicho = zSur - Largo;
        Vector2? hueco = null;
        var cascada = GameObject.Find("CascadaArena");
        if (cascada != null) hueco = new Vector2(cascada.transform.position.x, cascada.transform.position.z);

        TechoRect(prefab, grupo.transform, mats, xO, xE, zSur, zNorte, "Techo sala", hueco);            // sala
        TechoRect(prefab, grupo.transform, mats, xNichoO, xNichoE, zNicho, zSur, "Techo nicho", null);   // nicho de la esfinge
        // el tunel queda bajo (1 fila): su techo va a la altura de una pared
        TechoTunel(prefab, grupo.transform, mats, xTunO, xTunE, zNorte, -107.36f);
        // pozo de la cascada: una chimenea de 1 fila por encima del hueco, con tapa, y una luz calida adentro
        if (hueco.HasValue) Pozo(prefab, grupo.transform, mats, xO, xE, zSur, zNorte, hueco.Value);   // la cascada no se mueve: el pozo se arma donde esta
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(sala.scene);
        Debug.Log($"[Nivel Esfinge] techo armado a y {YPared + AltoPared * Filas:F3}" + (hueco.HasValue ? $", con hueco sobre la cascada ({hueco.Value.x:F1}, {hueco.Value.y:F1})" : ""));
    }

    [MenuItem("Tepet/Nivel Esfinge/Quitar techo de la sala")]
    static void QuitarTecho()
    {
        var sala = GameObject.Find("Sala SUBTERRANEA");
        var t = sala ? sala.transform.Find(GrupoTecho) : null;
        if (t) Undo.DestroyObjectImmediate(t.gameObject);
        if (sala) UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(sala.scene);
    }


    static GameObject Panel(GameObject prefab, Transform padre, Material[] mats, Vector3 pos, Quaternion rot, Vector3 escala, string nombre)
    {
        var go = (GameObject)PrefabUtility.InstantiatePrefab(prefab, padre);
        Undo.RegisterCreatedObjectUndo(go, "Techo sala esfinge");
        go.name = nombre; go.transform.position = pos; go.transform.rotation = rot; go.transform.localScale = escala;
        if (mats != null) foreach (var r in go.GetComponentsInChildren<Renderer>(true)) r.sharedMaterials = mats;
        return go;
    }

    static void TechoTunel(GameObject prefab, Transform padre, Material[] mats, float x0, float x1, float z0, float z1)
    {
        float sx = x1 - x0, sz = z1 - z0;
        Panel(prefab, padre, mats, new Vector3(x0, YPared + AltoPared, z0), Quaternion.Euler(0, 0, -90),
              new Vector3(EscalaBase, EscalaBase * (sx / AltoPared), EscalaBase * (sz / Largo)), "Techo tunel");
    }

    // Chimenea de LadoPozo x LadoPozo centrada sobre 'h': 4 paredes de 1 fila hacia arriba + tapa.
    static void Pozo(GameObject prefab, Transform padre, Material[] mats, float x0, float x1, float z0, float z1, Vector2 h)
    {
        float cx0 = Mathf.Clamp(h.x - LadoPozo / 2, x0 + 0.5f, x1 - LadoPozo - 0.5f), cx1 = cx0 + LadoPozo;
        float cz0 = Mathf.Clamp(h.y - LadoPozo / 2, z0 + 0.5f, z1 - LadoPozo - 0.5f), cz1 = cz0 + LadoPozo;
        float sx = cx1 - cx0, sz = cz1 - cz0;
        float yb = YPared + AltoPared * Filas;          // borde del techo
        float alto = AltoPared;                          // la chimenea sube una fila
        float k = alto / AltoPared;
        // paredes mirando hacia adentro del pozo (lado derecho del recorrido)
        Vector2[] pts = { new Vector2(cx0, cz1), new Vector2(cx1, cz1), new Vector2(cx1, cz0), new Vector2(cx0, cz0), new Vector2(cx0, cz1) };
        for (int s = 0; s < 4; s++)
        {
            Vector2 a = pts[s], b = pts[s + 1], d = b - a;
            float rotY = Mathf.Atan2(d.x, d.y) * Mathf.Rad2Deg;
            Panel(prefab, padre, mats, new Vector3(a.x, yb, a.y), Quaternion.Euler(0, rotY, 0),
                  new Vector3(EscalaBase, EscalaBase * k, EscalaBase * (d.magnitude / Largo)), $"Pozo cascada pared {s + 1}");
        }
        Panel(prefab, padre, mats, new Vector3(cx0, yb + alto, cz0), Quaternion.Euler(0, 0, -90),
              new Vector3(EscalaBase, EscalaBase * (sx / AltoPared), EscalaBase * (sz / Largo)), "Pozo cascada tapa");
        // luz calida adentro del pozo, para que la entrada de la arena se distinga
        var luz = new GameObject("Pozo cascada luz"); Undo.RegisterCreatedObjectUndo(luz, "Techo sala esfinge");
        luz.transform.SetParent(padre, true);
        luz.transform.position = new Vector3((cx0 + cx1) * 0.5f, yb + alto * 0.7f, (cz0 + cz1) * 0.5f);
        var l = luz.AddComponent<Light>(); l.type = LightType.Point; l.range = 9f; l.intensity = 2.5f; l.color = new Color(1f, 0.72f, 0.42f);
    }

    static string V(Vector3 v) => $"({v.x:F3}, {v.y:F3}, {v.z:F3})";
}
