using UnityEngine;

public class PermanentPainter : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Material drawMaterial; // Material del pincel
    [SerializeField] private RenderTexture paintMask; // Textura acumulativa
    [SerializeField] private float radius = 0.05f;

    private Vector2 lastUV;

    void Update()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.gameObject == gameObject)
        {
            lastUV = hit.textureCoord;

            drawMaterial.SetVector("_BrushUV", new Vector4(lastUV.x, lastUV.y, 0, 0));
            drawMaterial.SetFloat("_Radius", radius);

            // Crear textura temporal para mezclar
            RenderTexture temp = RenderTexture.GetTemporary(paintMask.width, paintMask.height, 0, RenderTextureFormat.ARGB32);
            Graphics.Blit(paintMask, temp); // copia lo anterior
            Graphics.Blit(temp, paintMask, drawMaterial); // aplica el nuevo trazo
            RenderTexture.ReleaseTemporary(temp);
        }
    }
}
