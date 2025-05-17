using JetBrains.Annotations;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class MousePainter : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private float radius = 0.1f;
    [SerializeField] private Texture2D brushTex; 
    private Renderer rend;
    public MaterialPropertyBlock propBlock;
    [SerializeField] private int textureSize = 1024;
    private RenderTexture paintMask;
    void Start()
    {
        rend = GetComponent<Renderer>();
        propBlock = new MaterialPropertyBlock();

        paintMask = new RenderTexture(textureSize, textureSize, 0, RenderTextureFormat.R8);
        paintMask.wrapMode = TextureWrapMode.Clamp;
        paintMask.filterMode = FilterMode.Bilinear;
        paintMask.name = "PaintMask";
        paintMask.Create();

        rend.GetPropertyBlock(propBlock);
        propBlock.SetTexture("_PaintMask", paintMask);
        rend.SetPropertyBlock(propBlock);
    }

    public void PaintAtUV(Vector2 uv)

    {
        RenderTexture.active = paintMask;

        GL.PushMatrix();
        GL.LoadPixelMatrix(0, paintMask.width, paintMask.height, 0);

        float px = uv.x * paintMask.width;
        float py = (1f - uv.y) * paintMask.height;

        Rect brushRect = new Rect(px - 10, py - 10, 20, 20); // 20 = tamaño del pincel

        Graphics.DrawTexture(brushRect, brushTex);

        GL.PopMatrix();
        RenderTexture.active = null;
    }
    void Update()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector2 uv = hit.textureCoord;
            if (hit.collider.gameObject == gameObject)
            {
                if (Input.GetMouseButton(0))
                    PaintAtUV(uv);

                rend.GetPropertyBlock(propBlock);
                propBlock.SetVector("_MouseUV", new Vector4(uv.x, uv.y, 0, 0));
                propBlock.SetFloat("_PaintRadius", radius);
                propBlock.SetFloat("_ShowPaint", 1);
                rend.SetPropertyBlock(propBlock);
                return;
            }
        }

        rend.GetPropertyBlock(propBlock);
        propBlock.SetFloat("_ShowPaint", 0);
        rend.SetPropertyBlock(propBlock);
    }
}
