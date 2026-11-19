using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class BrazoVivoTools
{
    [MenuItem("Tepet/Brazos/Agregar movimiento de reposo")]
    static void Agregar()
    {
        var hm = Object.FindFirstObjectByType<HandsManager>(FindObjectsInactive.Include);
        if (hm == null) { EditorUtility.DisplayDialog("Brazos", "No encuentro HandsManager en la escena.", "OK"); return; }
        var so = new SerializedObject(hm);
        int n = 0;
        foreach (var (campo, izq) in new[] { ("leftArm", true), ("rightArm", false) })
        {
            var t = so.FindProperty(campo)?.objectReferenceValue as Transform;
            if (t == null) continue;
            var bv = t.GetComponent<BrazoVivo>(); if (bv == null) bv = Undo.AddComponent<BrazoVivo>(t.gameObject);
            bv.esIzquierdo = izq; EditorUtility.SetDirty(bv); n++;
        }
        EditorSceneManager.MarkSceneDirty(hm.gameObject.scene);
        Debug.Log($"[Brazos] movimiento de reposo agregado a {n} brazos. Se ajusta en el componente 'Brazo Vivo'.");
    }

    [MenuItem("Tepet/Brazos/Quitar movimiento de reposo")]
    static void Quitar()
    {
        foreach (var bv in Object.FindObjectsByType<BrazoVivo>(FindObjectsInactive.Include, FindObjectsSortMode.None)) Undo.DestroyObjectImmediate(bv);
    }
}
