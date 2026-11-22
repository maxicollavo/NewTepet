using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// luces de la sala final: cuencos en la entrada, velas en el altar, el rayo del techo y poco mas
public static class LucesNivelFinal
{
    const string Grupo = "Iluminacion";
    const float Piso = -34.2f, Techo = -26.2f, Base = 0.7f;   // Base: alto de los escalones del altar
    static readonly Vector3 Altar = new Vector3(182f, Piso, -178f);
    static readonly Vector3 Hueco = new Vector3(171f, Techo, -170f);

    static List<Transform> Escena() =>
        Resources.FindObjectsOfTypeAll<Transform>().Where(t => t.gameObject.scene.IsValid() && !EditorUtility.IsPersistent(t)).ToList();

    [MenuItem("Tepet/Nivel Final/Poner luces")]
    static void Poner()
    {
        var todos = Escena();
        var raiz = todos.FirstOrDefault(t => t.name == "NivelFinal" && t.parent == null);
        if (raiz == null) { EditorUtility.DisplayDialog("Nivel final", "No encuentro NivelFinal en la escena.", "OK"); return; }
        var vieja = raiz.Find(Grupo); if (vieja) Undo.DestroyObjectImmediate(vieja.gameObject);
        todos = Escena().Where(t => t != null).ToList();

        var g = new GameObject(Grupo); Undo.RegisterCreatedObjectUndo(g, "Luces nivel final");
        g.transform.SetParent(raiz, false);

        // apagar las luces de trabajo
        var trabajo = raiz.Find("Luces"); if (trabajo) { Undo.RecordObject(trabajo.gameObject, "Luces nivel final"); trabajo.gameObject.SetActive(false); }

        // dos cuencos con brasas en la entrada (los de la sala de la iguana)
        var iguana = todos.FirstOrDefault(t => t.name == "LizardPuzzleRoom" && t.parent == null);
        var cuencos = iguana ? iguana.Find("Cuencos") : null;
        var cuenco = cuencos && cuencos.childCount > 0 ? cuencos.GetChild(0) : null;
        if (cuenco)
            foreach (var (n, x) in new[] { ("Cuenco", 176.2f), ("Cuenco (1)", 180.8f) })
            {
                var c = Object.Instantiate(cuenco.gameObject, g.transform); c.name = n; c.SetActive(true);
                float esc = 1f / Mathf.Max(cuenco.lossyScale.x, 0.01f);   // tamano de la sala 3 (en la iguana estan a 0.7)
                c.transform.localScale = cuenco.lossyScale * esc;
                foreach (var l in c.GetComponentsInChildren<Light>(true)) l.range *= esc;
                c.transform.position = new Vector3(x, cuenco.position.y, -167.3f);
                var rs = c.GetComponentsInChildren<Renderer>(true).Where(r => !(r is ParticleSystemRenderer)).ToArray();
                if (rs.Length > 0) c.transform.position += Vector3.up * (Piso - rs.Min(r => r.bounds.min.y));
            }
        else Debug.LogWarning("No encontre los cuencos de la iguana");

        // velas al pie del altar
        var velas = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Scenes/velas (2).prefab");
        if (velas)
        {
            var pos = new[] { new Vector2(180.7f, -175.0f), new Vector2(183.3f, -175.0f), new Vector2(180.7f, -181.0f), new Vector2(183.3f, -181.0f) };
            for (int k = 0; k < pos.Length; k++)
            {
                var v = (GameObject)PrefabUtility.InstantiatePrefab(velas, g.transform); v.name = k == 0 ? "velas" : $"velas ({k})";
                v.transform.position = new Vector3(pos[k].x, Piso + Base, pos[k].y);
                var rs = v.GetComponentsInChildren<Renderer>(true).Where(r => !(r is ParticleSystemRenderer)).ToArray();
                if (rs.Length > 0) v.transform.position += Vector3.up * (Piso + Base - rs.Min(r => r.bounds.min.y));
                var l = new GameObject("Point Light").AddComponent<Light>(); l.transform.SetParent(v.transform, false);
                l.transform.position = new Vector3(pos[k].x, Piso + Base + 0.5f, pos[k].y);
                l.type = LightType.Point; l.range = 6f; l.intensity = 3f; l.color = new Color(1f, 0.62f, 0.3f); l.lightmapBakeType = LightmapBakeType.Mixed;
            }
        }

        // el altar brilla un poco (emision dorada, no ilumina la sala)
        var oro = AssetDatabase.LoadAssetAtPath<Material>("Assets/Bauti/Materiales/OroMat.mat");
        if (oro) { Undo.RecordObject(oro, "Luces nivel final"); oro.EnableKeyword("_EMISSION"); oro.SetColor("_EmissionColor", new Color(1f, 0.7f, 0.25f) * 0.2f); oro.globalIlluminationFlags = MaterialGlobalIlluminationFlags.None; EditorUtility.SetDirty(oro); }

        // luz suave desde arriba sobre el ataud
        Spot(g.transform, "Luz ataud", new Vector3(Altar.x, Techo - 0.3f, Altar.z), 40f, 14f, 12f, new Color(1f, 0.85f, 0.6f));

        // relleno: dos luces calidas altas y suaves, no se ven como fuente
        foreach (var (n, p) in new[] { ("Relleno", new Vector3(176f, Techo - 1f, -172f)), ("Relleno (1)", new Vector3(188f, Techo - 1f, -183f)) })
        {
            var r = new GameObject(n).AddComponent<Light>(); r.transform.SetParent(g.transform, false); r.transform.position = p;
            r.type = LightType.Point; r.range = 16f; r.intensity = 1.6f; r.color = new Color(1f, 0.75f, 0.5f); r.shadows = LightShadows.None; r.lightmapBakeType = LightmapBakeType.Mixed;
        }

        // resplandor suave en el mural
        var luzMural = new GameObject("Luz mural"); luzMural.transform.SetParent(g.transform, false);
        luzMural.transform.SetPositionAndRotation(new Vector3(192.5f, Techo - 0.4f, -177f), Quaternion.Euler(60, 90, 0));
        var lm = luzMural.AddComponent<Light>(); lm.type = LightType.Spot; lm.spotAngle = 70; lm.innerSpotAngle = 30; lm.intensity = 6f; lm.range = 10f;
        lm.color = new Color(0.75f, 0.85f, 1f); lm.lightmapBakeType = LightmapBakeType.Mixed;

        // rayo del techo en las columnas
        Spot(g.transform, "Luz rayo", Hueco + Vector3.down * 0.3f, 14f, 30f, 12f, new Color(1f, 0.92f, 0.75f));
        var rayo = new GameObject("Rayo"); rayo.transform.SetParent(g.transform, false);
        rayo.transform.position = new Vector3(Hueco.x, (Techo + Piso) / 2f, Hueco.z);
        var mat = MatRayo();
        Cilindro(rayo.transform, "rayo", 0.3f, mat, 0.9f);
        Cilindro(rayo.transform, "rayo (1)", 0.55f, mat, 0.55f);
        Cilindro(rayo.transform, "rayo (2)", 0.85f, mat, 0.3f);

        // polvo en el aire (el de la sala 3)
        var polvo = todos.FirstOrDefault(t => t.name == "polvo de columnas");
        if (polvo)
        {
            // solo un emisor, centrado en el rayo (el original tiene varios repartidos por la sala 3)
            var ps = polvo.GetComponentsInChildren<ParticleSystem>(true).FirstOrDefault();
            if (ps)
            {
                var c = Object.Instantiate(ps.gameObject, rayo.transform); c.name = "polvo";
                foreach (Transform h in c.transform.Cast<Transform>().ToList()) Object.DestroyImmediate(h.gameObject);
                c.transform.position = new Vector3(Hueco.x, Piso + 3.5f, Hueco.z); c.SetActive(true);
            }
        }

        EditorSceneManager.MarkSceneDirty(raiz.gameObject.scene);
        Selection.activeGameObject = g;
        Debug.Log("Luces del nivel final puestas" + (cuenco ? "" : " (sin cuencos)") + (velas ? "" : " (sin velas)") + (polvo ? "" : " (sin polvo)"));
    }

    static void Spot(Transform padre, string nombre, Vector3 pos, float angulo, float intensidad, float rango, Color c)
    {
        var go = new GameObject(nombre); go.transform.SetParent(padre, false);
        go.transform.SetPositionAndRotation(pos, Quaternion.Euler(90, 0, 0));
        var l = go.AddComponent<Light>(); l.type = LightType.Spot; l.spotAngle = angulo; l.innerSpotAngle = angulo * 0.6f;
        l.intensity = intensidad; l.range = rango; l.color = c; l.shadows = LightShadows.Soft; l.lightmapBakeType = LightmapBakeType.Mixed;
    }

    static void Cilindro(Transform padre, string nombre, float radio, Material m, float alfa)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cylinder); go.name = nombre;
        Object.DestroyImmediate(go.GetComponent<Collider>());
        go.transform.SetParent(padre, false);
        go.transform.localScale = new Vector3(radio * 2f, (Techo - Piso) / 2f, radio * 2f);
        var r = go.GetComponent<MeshRenderer>(); r.sharedMaterial = m; r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; r.receiveShadows = false;
        var mpb = new MaterialPropertyBlock(); mpb.SetColor("_BaseColor", new Color(1f, 0.85f, 0.55f, 0.16f * alfa)); r.SetPropertyBlock(mpb);
    }

    static Material MatRayo()
    {
        const string ruta = "Assets/Bauti/Materiales/rayo luz.mat";
        var m = AssetDatabase.LoadAssetAtPath<Material>(ruta);
        if (m) return m;
        // textura: fuerte arriba, se apaga contra el piso
        var tex = new Texture2D(4, 64, TextureFormat.RGBA32, false) { wrapMode = TextureWrapMode.Clamp };
        for (int y = 0; y < 64; y++) { float a = Mathf.Pow(y / 63f, 1.6f); for (int x = 0; x < 4; x++) tex.SetPixel(x, y, new Color(1, 1, 1, a)); }
        tex.Apply();
        System.IO.File.WriteAllBytes("Assets/Bauti/Materiales/rayo.png", tex.EncodeToPNG());
        AssetDatabase.ImportAsset("Assets/Bauti/Materiales/rayo.png");
        var ti = (TextureImporter)AssetImporter.GetAtPath("Assets/Bauti/Materiales/rayo.png"); ti.alphaIsTransparency = true; ti.wrapMode = TextureWrapMode.Clamp; ti.SaveAndReimport();
        m = new Material(Shader.Find("Universal Render Pipeline/Particles/Unlit"));
        m.SetFloat("_Surface", 1f); m.SetFloat("_Blend", 2f);
        m.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha); m.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.One);
        m.SetFloat("_ZWrite", 0f); m.SetFloat("_Cull", 0f);
        m.EnableKeyword("_SURFACE_TYPE_TRANSPARENT"); m.EnableKeyword("_BLENDMODE_ADD");
        m.renderQueue = 3000;
        m.SetTexture("_BaseMap", AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Bauti/Materiales/rayo.png"));
        m.SetColor("_BaseColor", new Color(1f, 0.85f, 0.55f, 0.16f));
        AssetDatabase.CreateAsset(m, ruta);
        return m;
    }

    static GameObject Copiar(Transform src, Transform padre)
    {
        GameObject copia;
        var raizPrefab = PrefabUtility.GetOutermostPrefabInstanceRoot(src.gameObject);
        if (raizPrefab == src.gameObject)
        {
            var fuente = PrefabUtility.GetCorrespondingObjectFromSource(src.gameObject);
            copia = (GameObject)PrefabUtility.InstantiatePrefab(fuente, padre);
            var mods = PrefabUtility.GetPropertyModifications(src.gameObject);
            if (mods != null) PrefabUtility.SetPropertyModifications(copia, mods);
        }
        else copia = Object.Instantiate(src.gameObject, padre);
        copia.name = src.name.Split(" (")[0];
        copia.transform.SetPositionAndRotation(src.position, src.rotation);
        copia.transform.localScale = Vector3.Scale(src.lossyScale, new Vector3(1f / padre.lossyScale.x, 1f / padre.lossyScale.y, 1f / padre.lossyScale.z));
        copia.SetActive(true);
        return copia;
    }

    [MenuItem("Tepet/Nivel Final/Sacar luces")]
    static void Sacar()
    {
        var raiz = Escena().FirstOrDefault(t => t.name == "NivelFinal" && t.parent == null);
        if (!raiz) return;
        var g = raiz.Find(Grupo); if (g) Undo.DestroyObjectImmediate(g.gameObject);
        var trabajo = raiz.Find("Luces"); if (trabajo) { Undo.RecordObject(trabajo.gameObject, "Luces nivel final"); trabajo.gameObject.SetActive(true); }
        EditorSceneManager.MarkSceneDirty(raiz.gameObject.scene);
    }
}
