using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

// Nivel final "El Juicio de Maat" en gris (blockout) para que Maxi pueda programar.
// Queda al sur de la Sala de la Esfinge, conectada por un pasillo que sale detras del nicho de la esfinge.
public static class NivelFinalGris
{
    const string Raiz = "NivelFinal";
    const string CarpetaMat = "Assets/Bauti/Materiales/";
    const float Piso = -34.2f;

    static Material Mat(string nombre, Color c, Color? emision = null)
    {
        if (!AssetDatabase.IsValidFolder("Assets/Bauti/Materiales")) AssetDatabase.CreateFolder("Assets/Bauti", "Materiales");
        string ruta = CarpetaMat + nombre + ".mat";
        var m = AssetDatabase.LoadAssetAtPath<Material>(ruta);
        if (m != null) return m;
        m = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        m.SetColor("_BaseColor", c);
        m.SetFloat("_Smoothness", 0.2f);
        if (emision.HasValue) { m.EnableKeyword("_EMISSION"); m.SetColor("_EmissionColor", emision.Value); }
        AssetDatabase.CreateAsset(m, ruta);
        return m;
    }

    static Material gris, grisOsc, oro, lapis, sello;

    static GameObject Prim(PrimitiveType t, string nombre, Transform padre, Vector3 centro, Vector3 tam, Material m, Vector3? rot = null)
    {
        var go = GameObject.CreatePrimitive(t);
        go.name = nombre;
        go.transform.SetParent(padre, false);
        go.transform.position = centro;
        go.transform.rotation = Quaternion.Euler(rot ?? Vector3.zero);
        go.transform.localScale = tam;
        go.GetComponent<Renderer>().sharedMaterial = m;
        return go;
    }

    static Transform Grupo(string nombre, Transform padre) { var g = new GameObject(nombre); g.transform.SetParent(padre, false); return g.transform; }

    static void Etiqueta(Transform padre, string texto, Vector3 pos)
    {
        var go = new GameObject("Texto"); go.transform.SetParent(padre, false);
        go.transform.position = pos; go.transform.rotation = Quaternion.Euler(0, 180, 0);
        var tm = go.AddComponent<TextMesh>(); tm.text = texto; tm.characterSize = 0.18f; tm.fontSize = 48; tm.anchor = TextAnchor.MiddleCenter; tm.color = new Color(1f, 0.85f, 0.4f);
    }

    [MenuItem("Tepet/Nivel Final/Armar sala en gris")]
    static void Armar()
    {
        var viejo = GameObject.Find(Raiz);
        if (viejo != null) { EditorUtility.DisplayDialog("Nivel final", "Ya existe. Usa 'Quitar sala en gris' primero.", "OK"); return; }
        gris = Mat("Gris", new Color(0.55f, 0.52f, 0.48f));
        grisOsc = Mat("Gris oscuro", new Color(0.3f, 0.28f, 0.26f));
        oro = Mat("OroMat", new Color(0.85f, 0.65f, 0.25f));
        lapis = Mat("Azul", new Color(0.24f, 0.44f, 0.82f));
        sello = Mat("Sello", new Color(0.2f, 0.17f, 0.12f), new Color(0.6f, 0.4f, 0.1f) * 0.2f);

        var raiz = new GameObject(Raiz).transform;
        Undo.RegisterCreatedObjectUndo(raiz.gameObject, "Nivel final gris");

        // ---- Sala: x 168..196, z -165..-189, alto 8 ----
        var sala = Grupo("Sala", raiz);
        float x0 = 168, x1 = 196, zN = -165, zS = -189, alto = 8, e = 0.5f;
        float cx = (x0 + x1) / 2, cz = (zN + zS) / 2;
        Prim(PrimitiveType.Cube, "Piso", sala, new Vector3(cx, Piso - 0.25f, cz), new Vector3(x1 - x0, 0.5f, zN - zS), gris);
        Prim(PrimitiveType.Cube, "Pared", sala, new Vector3(cx, Piso + alto / 2, zS - e / 2), new Vector3(x1 - x0 + 2 * e, alto, e), grisOsc);
        Prim(PrimitiveType.Cube, "Pared (1)", sala, new Vector3(x0 - e / 2, Piso + alto / 2, cz), new Vector3(e, alto, zN - zS), grisOsc);
        Prim(PrimitiveType.Cube, "Pared (2)", sala, new Vector3(x1 + e / 2, Piso + alto / 2, cz), new Vector3(e, alto, zN - zS), grisOsc);
        // pared norte con la puerta (x 176.5..180.5)
        Prim(PrimitiveType.Cube, "Pared (3)", sala, new Vector3((x0 + 176.5f) / 2, Piso + alto / 2, zN + e / 2), new Vector3(176.5f - x0 + e, alto, e), grisOsc);
        Prim(PrimitiveType.Cube, "Pared (6)", sala, new Vector3((180.5f + x1) / 2, Piso + alto / 2, zN + e / 2), new Vector3(x1 - 180.5f + e, alto, e), grisOsc);
        Prim(PrimitiveType.Cube, "Dintel", sala, new Vector3(178.5f, Piso + 4.5f + (alto - 4.5f) / 2, zN + e / 2), new Vector3(4f, alto - 4.5f, e), grisOsc);
        Prim(PrimitiveType.Cube, "Techo", sala, new Vector3(cx, Piso + alto + e / 2, cz), new Vector3(x1 - x0 + 2 * e, e, zN - zS + 2 * e), grisOsc);

        // ---- Pasillo desde el nicho de la esfinge (z -146) hasta la sala ----
        var pas = Grupo("Pasillo", raiz);
        float pz0 = -146f, pz1 = zN, pl = pz0 - pz1, pc = (pz0 + pz1) / 2;
        Prim(PrimitiveType.Cube, "Piso (1)", pas, new Vector3(178.5f, Piso - 0.25f, pc), new Vector3(4f, 0.5f, pl), gris);
        Prim(PrimitiveType.Cube, "Pared (4)", pas, new Vector3(176.25f, Piso + 2.25f, pc), new Vector3(e, 4.5f, pl), grisOsc);
        Prim(PrimitiveType.Cube, "Pared (5)", pas, new Vector3(180.75f, Piso + 2.25f, pc), new Vector3(e, 4.5f, pl), grisOsc);
        Prim(PrimitiveType.Cube, "Techo (1)", pas, new Vector3(178.5f, Piso + 4.75f, pc), new Vector3(5f, e, pl), grisOsc);

        // ---- Altar dorado + ataud + sellos ----
        var alt = Grupo("Altar", raiz);
        Vector3 ac = new Vector3(182f, Piso, -178f);
        Prim(PrimitiveType.Cube, "altar", alt, ac + new Vector3(0, 0.6f, 0), new Vector3(3f, 1.2f, 5f), oro);
        var ataud = Prim(PrimitiveType.Cube, "Ataud", alt, ac + new Vector3(0, 1.65f, 0), new Vector3(1.3f, 0.9f, 3f), grisOsc);
        Prim(PrimitiveType.Cylinder, "Sello", alt, ac + new Vector3(-0.7f, 0.6f, 2.52f), new Vector3(0.6f, 0.03f, 0.6f), sello, new Vector3(90, 0, 0));
        Prim(PrimitiveType.Cylinder, "Sello (1)", alt, ac + new Vector3(0.7f, 0.6f, 2.52f), new Vector3(0.6f, 0.03f, 0.6f), sello, new Vector3(90, 0, 0));
        // 6 piedras traban la tapa: 3 de cada lado (I1..I3 oeste, D1..D3 este), en filas de norte a sur
        var piedras = Grupo("Piedras", alt);
        for (int lado = 0; lado < 2; lado++)
            for (int fila = 0; fila < 3; fila++)
                Prim(PrimitiveType.Cube, (lado == 0 ? "PiedraIzq" : "PiedraDer") + (fila > 0 ? " (" + fila + ")" : ""), piedras, ac + new Vector3(lado == 0 ? -0.38f : 0.38f, 2.25f, 1.1f - fila * 1.1f), new Vector3(0.55f, 0.25f, 0.6f), gris);

        // ---- Palancas: 3 a cada lado del ataud, una por piedra ----
        // Regla propuesta: cada palanca mueve su piedra, las vecinas de su lado y las dos en diagonal del otro lado.
        // Arranque: abajo I2 y D1. Solucion unica en 4: I1, I3, D2, D3.
        var pal = Grupo("Palancas", raiz);
        for (int lado = 0; lado < 2; lado++)
            for (int fila = 0; fila < 3; fila++)
            {
                var p = Grupo((lado == 0 ? "PalancaIzq" : "PalancaDer") + (fila > 0 ? " (" + fila + ")" : ""), pal);
                float sx = lado == 0 ? -1f : 1f;
                Vector3 b = ac + new Vector3(sx * 2.4f, 0, 1.1f - fila * 1.1f);
                Prim(PrimitiveType.Cube, "Base", p, b + new Vector3(0, 0.3f, 0), new Vector3(0.5f, 0.6f, 0.5f), gris);
                Prim(PrimitiveType.Cylinder, "Mango", p, b + new Vector3(sx * 0.15f, 0.95f, 0), new Vector3(0.1f, 0.4f, 0.1f), lapis, new Vector3(0, 0, sx * 25f));
                Prim(PrimitiveType.Sphere, "Pomo", p, b + new Vector3(sx * 0.33f, 1.32f, 0), Vector3.one * 0.18f, lapis);
            }

        // ---- Prueba 1: columnas con espejos (lado oeste) ----
        var col = Grupo("Columnas", raiz);
        Vector3[] cols = { new Vector3(171f, 0, -170f), new Vector3(174f, 0, -175f), new Vector3(170.5f, 0, -180f), new Vector3(174f, 0, -185f) };
        for (int i = 0; i < cols.Length; i++)
        {
            var c = Grupo("Columna" + (i > 0 ? " (" + i + ")" : ""), col);
            Vector3 b = new Vector3(cols[i].x, Piso, cols[i].z);
            Prim(PrimitiveType.Cylinder, "Fuste", c, b + new Vector3(0, 1.5f, 0), new Vector3(0.9f, 1.5f, 0.9f), gris);
            Prim(PrimitiveType.Cube, "Espejo", c, b + new Vector3(0, 3.35f, 0), new Vector3(0.9f, 0.7f, 0.06f), lapis, new Vector3(0, 45, 0));
        }
        Prim(PrimitiveType.Cylinder, "LuzTecho", col, new Vector3(171f, Piso + alto, -170f), new Vector3(1.2f, 0.3f, 1.2f), Mat("luz techo", Color.white, new Color(1f, 0.85f, 0.5f) * 3f));
        Prim(PrimitiveType.Cylinder, "Ojo de Ra", col, new Vector3(171f, Piso + 3.3f, zS + 0.05f), new Vector3(1.2f, 0.03f, 1.2f), oro, new Vector3(90, 0, 0));

        // ---- Prueba 2: esferas de los hijos de Horus (lado este) ----
        var esf = Grupo("Esferas", raiz);
        string[] dioses = { "Humano", "Babuino", "Chacal", "Halcon" };
        for (int i = 0; i < 4; i++)
        {
            Vector3 b = new Vector3(193.5f, Piso, -171f - i * 4f);
            Prim(PrimitiveType.Cube, "Pedestal" + (i > 0 ? " (" + i + ")" : ""), esf, b + new Vector3(0, 0.5f, 0), new Vector3(0.8f, 1f, 0.8f), gris);
            Prim(PrimitiveType.Sphere, "Esfera " + dioses[i].ToLower(), esf, b + new Vector3(0, 1.3f, 0), Vector3.one * 0.5f, lapis);
        }
        Prim(PrimitiveType.Quad, "Mural", esf, new Vector3(x1 - 0.02f, Piso + 3.5f, cz), new Vector3(8f, 3.5f, 1f), Mat("MuralMat", new Color(0.7f, 0.6f, 0.4f)), new Vector3(0, 90, 0));
        var bases = Grupo("Bases", esf);
        var puntos = new (string n, Vector3 o)[] { ("Norte", new Vector3(0, 0, 4.5f)), ("Sur", new Vector3(0, 0, -4.5f)), ("Este", new Vector3(4.6f, 0, 0)), ("Oeste", new Vector3(-4.6f, 0, 0)) };
        foreach (var (n, o) in puntos) Prim(PrimitiveType.Cylinder, "Base" + n, bases, ac + o + new Vector3(0, 0.2f, 0), new Vector3(0.9f, 0.2f, 0.9f), gris);

        // ---- Etiquetas para orientarse en el editor ----
        var et = Grupo("Textos", raiz);
        Etiqueta(et, "COLUMNAS", new Vector3(172f, Piso + 5f, -177f));
        Etiqueta(et, "ESFERAS", new Vector3(193.5f, Piso + 5f, -177f));
        Etiqueta(et, "PALANCAS", new Vector3(182f, Piso + 3.4f, -175f));

        // ---- Luz de trabajo ----
        var luces = Grupo("Luces", raiz);
        foreach (var p in new[] { new Vector3(173f, Piso + 6f, -170f), new Vector3(191f, Piso + 6f, -170f), new Vector3(173f, Piso + 6f, -184f), new Vector3(191f, Piso + 6f, -184f), new Vector3(178.5f, Piso + 3.5f, -155f) })
        {
            var l = new GameObject("Point Light").AddComponent<Light>(); l.transform.SetParent(luces, false); l.transform.position = p;
            l.type = LightType.Point; l.range = 14f; l.intensity = 2.2f; l.color = new Color(1f, 0.78f, 0.55f); l.lightmapBakeType = LightmapBakeType.Mixed; l.shadows = LightShadows.None;
        }

        EditorSceneManager.MarkSceneDirty(raiz.gameObject.scene);
        Selection.activeGameObject = raiz.gameObject;
        Debug.Log("[Nivel final] sala en gris armada al sur de la Esfinge (x 168..196, z -165..-189, piso -34.2). El pasillo arranca detras del nicho de la esfinge (z -146).");
    }

    [MenuItem("Tepet/Nivel Final/Quitar sala en gris")]
    static void Quitar()
    {
        var g = GameObject.Find(Raiz);
        if (g) { var esc = g.scene; Undo.DestroyObjectImmediate(g); EditorSceneManager.MarkSceneDirty(esc); }
    }
    // Reemplaza los cilindros por copias visuales de las columnas de la sala 3 (sin sus scripts),
    // y sube los espejos arriba de cada columna.
    [MenuItem("Tepet/Nivel Final/Poner columnas de la sala 3")]
    static void PonerColumnas()
    {
        var raiz = GameObject.Find(Raiz);
        if (raiz == null) { EditorUtility.DisplayDialog("Nivel final", "Primero arma la sala en gris.", "OK"); return; }
        Transform sala3 = null;
        foreach (var r in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects()) if (r.name == "Sala 3") sala3 = r.transform;
        var modelos = new[] { sala3 ? sala3.Find("LeftBackColumn") : null, sala3 ? sala3.Find("RightBackColumn") : null };
        if (modelos[0] == null || modelos[1] == null) { EditorUtility.DisplayDialog("Nivel final", "No encuentro las columnas de la sala 3.", "OK"); return; }
        var grupo = raiz.transform.Find("Columnas");
        int n = 0;
        foreach (Transform c in grupo)
        {
            if (!c.name.StartsWith("Columna")) continue;
            var fuste = c.Find("Fuste");
            var espejo = c.Find("Espejo");
            var vieja = c.Find("ColumnaSala3"); if (vieja) Undo.DestroyObjectImmediate(vieja.gameObject);
            Vector3 pos = fuste ? new Vector3(fuste.position.x, Piso, fuste.position.z) : c.position;
            var modelo = modelos[n % 2];
            var copia = Object.Instantiate(modelo.gameObject, c);
            Undo.RegisterCreatedObjectUndo(copia, "Columnas");
            copia.name = "ColumnaSala3";
            foreach (var mb in copia.GetComponentsInChildren<MonoBehaviour>(true)) Object.DestroyImmediate(mb);
            foreach (var a in copia.GetComponentsInChildren<Animator>(true)) Object.DestroyImmediate(a);
            copia.SetActive(true);
            // apoyar en el piso y centrar
            var rs = copia.GetComponentsInChildren<Renderer>(true);
            if (rs.Length > 0)
            {
                var b = rs[0].bounds; foreach (var r in rs) b.Encapsulate(r.bounds);
                copia.transform.position += new Vector3(pos.x - b.center.x, Piso - b.min.y, pos.z - b.center.z);
                b = rs[0].bounds; foreach (var r in rs) b.Encapsulate(r.bounds);
                if (espejo) { Undo.RecordObject(espejo, "Columnas"); espejo.position = new Vector3(pos.x, b.max.y + 0.4f, pos.z); }
            }
            if (fuste) Undo.DestroyObjectImmediate(fuste.gameObject);
            n++;
        }
        EditorSceneManager.MarkSceneDirty(raiz.scene);
        Debug.Log($"[Nivel final] {n} columnas de la sala 3 puestas (sin scripts), con el espejo arriba.");
    }
}
