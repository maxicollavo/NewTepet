using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public static class ArmarUIPuzzles
{
    const string Raiz = "Assets/Art/2D/";

    [MenuItem("Tepet/Armar UI anillos y jeroglificos")]
    static void Armar()
    {
        var rio = Resources.FindObjectsOfTypeAll<Transform>().FirstOrDefault(t => t.gameObject.scene.IsValid() && t.parent == null && t.name == "tutorialRioPuzzle");
        if (!rio) { Debug.LogWarning("no encuentro tutorialRioPuzzle"); return; }

        // ---- anillos: click cambia de anillo, A y D rotan, click derecho vuelve
        var anillos = Copia(rio, "tutorialAnillos");
        var wasd = Hijo(anillos, "Move the Pieces");
        Poner(wasd, Raiz + "UI CONTROLES COLUMNAS/A Y D (ROTAR)/A y D ROTATE (Juntos).png", 400f, 50f);
        foreach (var c in Object.FindObjectsByType<RingPuzzleController>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            Asignar(c, "uiRingPuzzle", anillos);

        // ---- jeroglificos: click para dibujar, click derecho vuelve
        var jero = Copia(rio, "tutorialJeroglificos");
        Undo.DestroyObjectImmediate(Hijo(jero, "Move the Pieces").gameObject);
        Undo.DestroyObjectImmediate(Hijo(jero, "Solo CHANGE SELECTION").gameObject);
        var mouse = Hijo(jero, "MOUSE CLICKED");
        var texto = new GameObject("Image (1)", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        Undo.RegisterCreatedObjectUndo(texto, "UI");
        texto.transform.SetParent(jero.transform, false);
        var rtM = (RectTransform)mouse.transform;
        var rt = (RectTransform)texto.transform; rt.anchorMin = rtM.anchorMin; rt.anchorMax = rtM.anchorMax;
        rt.anchoredPosition = new Vector2(0, rtM.anchoredPosition.y);
        float w = Poner(texto.transform, Raiz + "UI MENUS 20-4-25/USE POWERS/LEFT MOUSE CLICK.png", 0f, 40f);
        rt.anchoredPosition = new Vector2(rtM.anchoredPosition.x - 40f - w / 2f, rtM.anchoredPosition.y);
        foreach (var m in Object.FindObjectsByType<TrackerManager>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            Asignar(m, "uiHieroglyph", jero);

        EditorSceneManager.MarkSceneDirty(rio.gameObject.scene);
        Debug.Log("UI de anillos y jeroglificos armada");
    }

    [MenuItem("Tepet/Ver u ocultar UI anillos")] static void VerA() => Alternar("tutorialAnillos");
    [MenuItem("Tepet/Ver u ocultar UI jeroglificos")] static void VerJ() => Alternar("tutorialJeroglificos");
    static void Alternar(string n)
    {
        var g = Resources.FindObjectsOfTypeAll<Transform>().FirstOrDefault(t => t.gameObject.scene.IsValid() && t.parent == null && t.name == n);
        if (g) g.gameObject.SetActive(!g.gameObject.activeSelf);
        UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
    }

    static GameObject Copia(Transform src, string nombre)
    {
        var vieja = Resources.FindObjectsOfTypeAll<Transform>().FirstOrDefault(t => t.gameObject.scene.IsValid() && t.parent == null && t.name == nombre);
        if (vieja) Undo.DestroyObjectImmediate(vieja.gameObject);
        var c = Object.Instantiate(src.gameObject); c.name = nombre; c.SetActive(false);
        c.transform.SetSiblingIndex(src.GetSiblingIndex() + 1);
        Undo.RegisterCreatedObjectUndo(c, "UI");
        return c;
    }

    static Transform Hijo(GameObject g, string sprite) =>
        g.GetComponentsInChildren<Image>(true).First(i => i.sprite && i.sprite.name.Contains(sprite)).transform;

    // pone el sprite con su proporcion real, alto en pixeles, y lo alinea a la izquierda desde x
    static float Poner(Transform t, string ruta, float x, float alto)
    {
        var img = t.GetComponent<Image>();
        var sp = AssetDatabase.LoadAssetAtPath<Sprite>(ruta);
        if (!sp)
        {
            // la imagen no estaba importada como Sprite (UI)
            var ti = AssetImporter.GetAtPath(ruta) as TextureImporter;
            if (ti) { ti.textureType = TextureImporterType.Sprite; ti.spriteImportMode = SpriteImportMode.Single; ti.alphaIsTransparency = true; ti.SaveAndReimport(); }
            sp = AssetDatabase.LoadAssetAtPath<Sprite>(ruta);
        }
        if (!sp) { Debug.LogWarning("no encuentro " + ruta); return 0f; }
        img.sprite = sp; img.preserveAspect = true;
        var rt = (RectTransform)t;
        float k = alto / sp.rect.height;
        rt.sizeDelta = new Vector2(sp.rect.width, sp.rect.height);
        rt.localScale = new Vector3(k, k, 1f);
        rt.anchoredPosition = new Vector2(x + sp.rect.width * k / 2f, rt.anchoredPosition.y);
        return sp.rect.width * k;
    }

    static void Asignar(Object o, string campo, GameObject ui)
    {
        var so = new SerializedObject(o);
        var p = so.FindProperty(campo); if (p == null) return;
        p.objectReferenceValue = ui; so.ApplyModifiedProperties();
    }
}
