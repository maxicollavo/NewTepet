// Tepet's Pyramid - Visual de la arena dentro de una vasija (solo visual, no toca la logica).
// Va en el objeto de relleno (hijo "ArenaRelleno" con M_ArenaVasija) dentro de la vasija.
// Desde la logica alcanza con: GetComponentInChildren<VasijaArenaVisual>(true)?.SumarNivel();
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class VasijaArenaVisual : MonoBehaviour
{
    [Tooltip("Cuantos niveles de arena entran en esta vasija")]
    public int nivelesMaximos = 5;   // las placas van de I a IIIII
    [Range(0f, 1f)] public float llenado = 0f;      // valor inicial / para probar en el Inspector
    [Tooltip("Segundos que tarda la arena en subir o bajar un nivel completo")]
    public float duracion = 0.6f;

    static readonly int FillId = Shader.PropertyToID("_Fill");
    static readonly int MinId = Shader.PropertyToID("_MinY");
    static readonly int MaxId = Shader.PropertyToID("_MaxY");
    float minY = -1f, maxY = 1f;
    Renderer rend;
    MaterialPropertyBlock block;
    float actual, objetivo;
    int nivel;
    bool listo;

    public int Nivel => nivel;

    void Init()
    {
        if (listo) return;
        rend = GetComponent<Renderer>();
        block = new MaterialPropertyBlock();
        // la altura de corte se toma del mesh (sirve para cubo, cilindro o la forma interior de la vasija)
        var mf = GetComponent<MeshFilter>();
        if (mf != null && mf.sharedMesh != null) { minY = mf.sharedMesh.bounds.min.y; maxY = mf.sharedMesh.bounds.max.y; }
        actual = objetivo = llenado;
        nivel = Mathf.RoundToInt(llenado * nivelesMaximos);
        listo = true;
    }

    void Awake() { Init(); Aplicar(); }
    void OnEnable() { Init(); Aplicar(); }

    /// Suma un nivel de arena (lo llama la cascada en cada interaccion).
    public void SumarNivel() { Init(); SetNivel(nivel + 1, nivelesMaximos); }

    /// Saca un nivel de arena.
    public void QuitarNivel() { Init(); SetNivel(nivel - 1, nivelesMaximos); }

    /// Vacia la vasija.
    public void Vaciar() { Init(); SetNivel(0, nivelesMaximos); }

    /// nivel = cuantos niveles tiene, maximo = cuantos entran.
    public void SetNivel(int nuevoNivel, int maximo)
    {
        Init();
        nivelesMaximos = Mathf.Max(1, maximo);
        nivel = Mathf.Clamp(nuevoNivel, 0, nivelesMaximos);
        objetivo = nivel / (float)nivelesMaximos;
        if (!isActiveAndEnabled) { actual = objetivo; Aplicar(); }   // si esta oculta, que aparezca ya en su nivel
    }

    /// Llenado directo entre 0 y 1.
    public void SetLlenado(float valor01) { Init(); objetivo = Mathf.Clamp01(valor01); }

    void Update()
    {
        if (Mathf.Approximately(actual, objetivo)) return;
        actual = Mathf.MoveTowards(actual, objetivo, Time.deltaTime / Mathf.Max(duracion, 0.01f));
        Aplicar();
    }

    void OnValidate()
    {
        // en el editor, mover el slider "llenado" muestra el resultado sin dar Play
        if (Application.isPlaying) { if (listo) objetivo = llenado; return; }
        listo = false; Init(); Aplicar();
    }

    void Aplicar()
    {
        if (rend == null) return;
        rend.GetPropertyBlock(block);
        block.SetFloat(FillId, actual);
        block.SetFloat(MinId, minY);
        block.SetFloat(MaxId, maxY);
        rend.SetPropertyBlock(block);
    }
}
