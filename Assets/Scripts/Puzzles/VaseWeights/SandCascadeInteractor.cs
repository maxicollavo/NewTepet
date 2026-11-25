using UnityEngine;

public class SandCascadeInteractor : MonoBehaviour, Interactor
{
    [SerializeField] private VaseWeightManager vaseManager;
    private Outline outline;

    [Header("Inclinar el jarron al acercarse")]
    [SerializeField] private float radioInclinar = 3.5f;
    private BrazoVivo brazo;

    void Awake()
    {
        if (outline == null)
        {
            outline = GetComponent<Outline>();
        }
    }

    private void Start()
    {
        if (outline != null)
        {
            outline.enabled = false;
        }
    }

    // cerca de la cascada con un jarron en la mano: el brazo gira para dejar la boca del jarron para arriba
    private void Update()
    {
        HandVases enMano = null;
        if (HandInventory.IsHoldingSomething())
            foreach (var h in FindObjectsByType<HandVases>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)) { enMano = h; break; }

        bool cerca = false;
        if (enMano != null && Camera.main != null)
        {
            Vector3 a = Camera.main.transform.position, b = transform.position;
            a.y = b.y = 0f;
            cerca = Vector3.Distance(a, b) < radioInclinar;
        }

        var nuevo = enMano ? enMano.GetComponentInParent<BrazoVivo>() : null;
        if (brazo != null && brazo != nuevo) { brazo.inclinacionObjetivo = 0f; }
        brazo = nuevo;
        if (brazo != null)
        {
            brazo.objetoArriba = enMano.transform;
            brazo.inclinacionObjetivo = cerca ? 1f : 0f;
        }
    }

    public void Interact()
    {
        if (vaseManager == null)
        {
            Debug.LogWarning("No hay un VaseWeightManager asignado.");
            return;
        }

        // solo se llena parado en la cascada y con un jarron en la mano
        if (!HandInventory.IsHoldingSomething() || brazo == null || brazo.inclinacionObjetivo < 0.5f) return;

        vaseManager.TryFillVase();
        AudioManager.Instance?.PlaySound("LlenadoVasija", 0.8f);
        DisableOutline();
    }

    public void Aiming()
    {
        if (outline != null)
        {
            outline.enabled = true;
        }
    }

    public void DisableOutline()
    {
        if (outline != null)
        {
            outline.enabled = false;
        }
    }
}
