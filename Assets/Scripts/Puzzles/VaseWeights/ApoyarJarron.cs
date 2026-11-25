using System.Collections;
using UnityEngine;

// Va en cada placa de presion. Con un jarron en la mano, click en la placa y el jarron queda apoyado ahi.
// El jarron del piso es el mismo de siempre (Vase), asi se puede volver a agarrar y mantiene su arena.
public class ApoyarJarron : MonoBehaviour, Interactor
{
    [SerializeField] private float alturaExtra = 0.02f;
    private GameObject jarronApoyado;
    private Outline outline;
    private Collider coll;

    public GameObject JarronApoyado => jarronApoyado != null && jarronApoyado.activeInHierarchy ? jarronApoyado : null;

    private void Awake()
    {
        outline = GetComponent<Outline>();
        coll = GetComponent<Collider>();
        if (outline) outline.enabled = false;
    }

    private bool TieneJarronEnMano(out ObjectsToPick obj)
    {
        obj = PickedObjData.Instance ? PickedObjData.Instance.CurrentPickedObj : ObjectsToPick.None;
        return obj == ObjectsToPick.Vase0 || obj == ObjectsToPick.Vase1 || obj == ObjectsToPick.Vase2 || obj == ObjectsToPick.Vase3;
    }

    public void Aiming()
    {
        if (!TieneJarronEnMano(out _)) return;
        if (outline) outline.enabled = true;
        UIManager.Instance.ChangeCursor(true);
    }

    public void DisableOutline()
    {
        if (outline) outline.enabled = false;
        UIManager.Instance.ChangeCursor(false);
    }

    public void Interact()
    {
        if (!TieneJarronEnMano(out var obj)) return;
        if (JarronApoyado != null) { StartCoroutine(Ocupada()); return; }

        int indice = obj - ObjectsToPick.Vase0;
        Vase vase = null;
        foreach (var v in VaseWeightManager.Instance.vases) if (v != null && v.vaseIndex == indice) vase = v;
        if (vase == null) { Debug.LogWarning("No encuentro el jarron del piso con indice " + indice); return; }

        // apoyarlo arriba de la placa, parado
        Bounds b = coll ? coll.bounds : new Bounds(transform.position, Vector3.zero);
        var t = vase.transform;
        t.rotation = Quaternion.Euler(0f, t.eulerAngles.y, 0f);
        t.gameObject.SetActive(true);
        var vb = Limites(t.gameObject);
        t.position += new Vector3(b.center.x - vb.center.x, b.max.y + alturaExtra - vb.min.y, b.center.z - vb.center.z);
        jarronApoyado = t.gameObject;

        // sacar el de la mano
        PickedObjData.Instance.MarkAsThrowed(obj, true);
        DisableOutline();
        AudioManager.Instance?.PlaySound("PlacaPiso", 0.8f);
    }

    private IEnumerator Ocupada()
    {
        if (!outline) yield break;
        var c = outline.OutlineColor;
        outline.enabled = true; outline.OutlineColor = Color.red;
        yield return new WaitForSeconds(0.5f);
        outline.OutlineColor = c; outline.enabled = false;
    }

    private static Bounds Limites(GameObject g)
    {
        var rs = g.GetComponentsInChildren<Renderer>();
        Bounds b = new Bounds(g.transform.position, Vector3.zero); bool hay = false;
        foreach (var r in rs) { if (!r.enabled) continue; if (!hay) { b = r.bounds; hay = true; } else b.Encapsulate(r.bounds); }
        if (!hay) { var c = g.GetComponent<Collider>(); if (c) b = c.bounds; }
        return b;
    }
}
