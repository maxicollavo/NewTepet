using UnityEngine;

public class LightBlinker : MonoBehaviour
{
    [Header("Luces a parpadear")]
    public Light[] luces;

    [Header("Configuración del parpadeo")]
    [Tooltip("Tiempo entre cada cambio de estado")]
    public float intervalo = 0.5f;

    [Tooltip("Variación aleatoria en el intervalo (para hacerlo más natural)")]
    public float variacion = 0.2f;

    private float timer;

    void Start()
    {
        timer = intervalo;
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            // Cambiar el estado de cada luz
            foreach (Light l in luces)
            {
                if (l != null)
                    l.enabled = !l.enabled;
            }

            // Reiniciar el temporizador con algo de aleatoriedad
            timer = intervalo + Random.Range(-variacion, variacion);
        }
    }
}