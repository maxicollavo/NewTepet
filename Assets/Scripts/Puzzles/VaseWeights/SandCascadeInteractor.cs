using UnityEngine;

public class SandCascadeInteractor : MonoBehaviour, Interactor
{
    [SerializeField] private VaseWeightManager vaseManager;
    private Outline outline;

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

    public void Interact()
    {
        if (vaseManager == null)
        {
            Debug.LogWarning("No hay un VaseWeightManager asignado.");
            return;
        }

        vaseManager.TryFillVase();
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
