using UnityEngine;

public class EmissiveLightController : MonoBehaviour
{
    public Renderer targetRenderer;
    public Light controlledLight;
    private Material shaderGraphMaterial;

    public float frequency = 1f; // 1 ciclo cada 2 * pi segundos (~6.28s)
    public float threshold = 0.5f; // punto de encendido

    void Start()
    {
        Material[] materials = targetRenderer.materials;
        foreach (var mat in materials)
        {
            if (mat.HasProperty("_Timer"))
            {
                shaderGraphMaterial = mat;
                break;
            }
        }
    }

    void Update()
    {
        // Avanza el tiempo, aplicando seno, normalizado entre 0 y 1
        float rawSin = Mathf.Sin(Time.time * frequency);
        float timeValue = rawSin * 0.5f + 0.5f; // convierte -1..1 en 0..1

        // Pasarlo al shader
        shaderGraphMaterial.SetFloat("_Timer", timeValue);

        // Enciende la luz si el valor supera el umbral
        controlledLight.enabled = timeValue > threshold;
    }
}