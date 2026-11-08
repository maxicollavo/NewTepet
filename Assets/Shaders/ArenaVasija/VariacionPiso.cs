// Tepet's Pyramid - valores distintos por baldosa sin duplicar el material.
// Usa MaterialPropertyBlock: cada objeto puede tener su propio relieve, tono y desfase de textura
// mientras todos comparten el mismo material (PisoCueva).
using UnityEngine;

[ExecuteAlways]
[RequireComponent(typeof(Renderer))]
public class VariacionPiso : MonoBehaviour
{
    [Tooltip("Altura del relieve (Parallax). 0 = usar el del material")]
    [Range(0f, 0.08f)] public float relieve = 0f;
    [Tooltip("Fuerza del normal map. 0 = usar el del material")]
    [Range(0f, 3f)] public float normal = 0f;
    [Tooltip("Tinte de esta baldosa (blanco = sin cambio)")]
    public Color tinte = Color.white;
    [Tooltip("Repeticion de la textura")]
    public Vector2 tiling = Vector2.one;
    [Tooltip("Desfase de la textura, para que las baldosas no se vean iguales")]
    public Vector2 desfase = Vector2.zero;

    static readonly int ParallaxId = Shader.PropertyToID("_Parallax");
    static readonly int BumpId = Shader.PropertyToID("_BumpScale");
    static readonly int ColorId = Shader.PropertyToID("_BaseColor");
    static readonly int STId = Shader.PropertyToID("_BaseMap_ST");
    MaterialPropertyBlock block;

    void OnEnable() { Aplicar(); }
    void OnValidate() { Aplicar(); }

    public void Aplicar()
    {
        var r = GetComponent<Renderer>();
        if (r == null) return;
        if (block == null) block = new MaterialPropertyBlock();
        r.GetPropertyBlock(block);
        block.Clear();
        var mat = r.sharedMaterial;
        if (relieve > 0f) block.SetFloat(ParallaxId, relieve);
        if (normal > 0f) block.SetFloat(BumpId, normal);
        if (mat != null && mat.HasProperty(ColorId)) block.SetColor(ColorId, mat.GetColor(ColorId) * tinte);
        block.SetVector(STId, new Vector4(tiling.x, tiling.y, desfase.x, desfase.y));
        r.SetPropertyBlock(block);
    }

    void OnDisable()
    {
        var r = GetComponent<Renderer>();
        if (r != null) r.SetPropertyBlock(null);
    }
}
