using UnityEngine;

// movimiento de los brazos cuando estas quieto:
// - una respiracion muy leve todo el tiempo
// - cada tanto, un gesto corto (mirar la mano, estirar, mover la muneca)
// Se suma encima de lo que hagan HandsManager o el Animator, sin pisarlos,
// y se apaga solo cuando el brazo esta en pose de puzzle.
[DefaultExecutionOrder(-1000)]
public class BrazoVivo : MonoBehaviour
{
    [Header("Respiracion")]
    public float respiracion = 0.5f;          // grados
    public float ritmo = 0.22f;               // ciclos por segundo

    [Header("Gestos")]
    public Vector2 cadaCuanto = new Vector2(9f, 16f); // segundos entre gestos
    public float intensidad = 1f;
    public bool esIzquierdo;

    // inclinacion pedida desde afuera: deja "objetoArriba" con su eje Y mirando para arriba
    [HideInInspector] public Transform objetoArriba;
    [HideInInspector] public float inclinacionObjetivo;
    [Tooltip("Que tan rapido se inclina (1 = un segundo)")] public float velocidadInclinacion = 2f;
    [Tooltip("Maximo que se inclina el brazo")] public float inclinacionMaxima = 50f;
    float inclinacion;

    Quaternion reposo, ultimoOffset = Quaternion.identity;
    bool inicializado;
    float proximoGesto, gestoInicio = -100f, gestoDur;
    int gesto;
    float fase, peso = 1f;

    void Start()
    {
        reposo = transform.localRotation;
        fase = esIzquierdo ? 1.7f : 0f;
        proximoGesto = Time.time + Random.Range(cadaCuanto.x, cadaCuanto.y);
        inicializado = true;
    }

    // Al principio de cada frame sacamos el offset del frame anterior, asi los otros scripts
    // y el Animator trabajan sobre la rotacion limpia y nunca se acumula nada.
    void Update()
    {
        if (!inicializado) return;
        transform.localRotation = transform.localRotation * Quaternion.Inverse(ultimoOffset);
        ultimoOffset = Quaternion.identity;
    }

    void OnDisable()
    {
        if (!inicializado) return;
        transform.localRotation = transform.localRotation * Quaternion.Inverse(ultimoOffset);
        ultimoOffset = Quaternion.identity;
    }

    void LateUpdate()
    {
        if (!inicializado) return;
        Quaternion baseRot = transform.localRotation;

        // En pose de puzzle (brazo lejos de su reposo) nos apagamos de a poco
        bool enPuzzle = Quaternion.Angle(baseRot, reposo) > 12f;
        bool sosteniendo = HandInventory.IsHoldingSomething();
        float objetivo = enPuzzle ? 0f : (sosteniendo ? 0.5f : 1f);
        peso = Mathf.MoveTowards(peso, objetivo, Time.deltaTime * 2f);

        float t = Time.time;
        Vector3 e = new Vector3(Mathf.Sin((t + fase) * ritmo * 2f * Mathf.PI) * respiracion,
                                Mathf.Sin((t + fase) * ritmo * 1.3f * Mathf.PI) * respiracion * 0.4f, 0f);

        if (!enPuzzle && t >= proximoGesto)
        {
            gesto = Random.Range(0, 3);
            gestoDur = gesto == 2 ? 1.4f : 1.8f;
            gestoInicio = t;
            proximoGesto = t + gestoDur + Random.Range(cadaCuanto.x, cadaCuanto.y);
        }
        float u = (t - gestoInicio) / gestoDur;
        if (u >= 0f && u <= 1f)
        {
            float campana = Mathf.Sin(u * Mathf.PI);              // sube y vuelve
            float lado = esIzquierdo ? -1f : 1f;
            switch (gesto)
            {
                case 0: e += new Vector3(-4f, 2f * lado, 4f * lado) * campana; break;       // mirar la mano
                case 1: e += new Vector3(3f, -2f * lado, -2f * lado) * campana; break;      // estirar
                default: e += new Vector3(0f, 0f, Mathf.Sin(u * Mathf.PI * 4f) * 2.5f * lado) * campana; break; // mover la muneca
            }
        }

        Quaternion offset = Quaternion.Euler(e * intensidad * peso);

        // jarron con la boca para arriba (cerca de la cascada)
        inclinacion = Mathf.MoveTowards(inclinacion, objetoArriba ? inclinacionObjetivo : 0f, Time.deltaTime * velocidadInclinacion);
        if (inclinacion > 0.001f && objetoArriba)
        {
            Quaternion delta = Quaternion.FromToRotation(objetoArriba.up, Vector3.up);
            delta.ToAngleAxis(out float ang, out Vector3 eje);
            if (ang > 180f) ang -= 360f;
            ang = Mathf.Clamp(ang, -inclinacionMaxima, inclinacionMaxima) * Mathf.SmoothStep(0f, 1f, inclinacion);
            Quaternion mundo = Quaternion.AngleAxis(ang, eje);
            Quaternion rot = transform.rotation;
            offset = (Quaternion.Inverse(rot) * mundo * rot) * offset;
        }

        transform.localRotation = baseRot * offset;
        ultimoOffset = offset;
    }
}
