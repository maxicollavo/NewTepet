using UnityEngine;

public class MousePainter : MonoBehaviour
{
    public Material material;
    public float brushSize = 0.1f;

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // Obtener UV del objeto en el punto impactado
            Vector2 uv = hit.textureCoord;

            // Enviar al shader
            material.SetVector("_MouseUV", uv);
            material.SetFloat("_BrushSize", brushSize);
        }
        else
        {
            // Si no está encima, envía un valor imposible para que no pinte
            material.SetVector("_MouseUV", new Vector2(-1, -1));
        }
    }
}