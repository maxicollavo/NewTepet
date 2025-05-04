using UnityEngine;

public class ActivateLasers : MonoBehaviour, Interactor
{
    [SerializeField] LineRenderer lineRenderer1;
    [SerializeField] LineRenderer lineRenderer2;
    [SerializeField] WallLaser wl1;
    [SerializeField] WallLaser wl2;

    Outline outline;
    BoxCollider boxCol;
    Animator anim;

    private void Awake()
    {
        outline = GetComponent<Outline>();
        boxCol = GetComponent<BoxCollider>();
        anim = GetComponent<Animator>();

        outline.enabled = false;
    }

    public void DisableOutline()
    {
        outline.enabled = false;

        UIManager.Instance.ChangeCursor(false);
    }

    void EnableOutline()
    {
        outline.enabled = true;
    }

    public void Aiming()
    {
        EnableOutline();

        UIManager.Instance.ChangeCursor(true);
    }

    public void Interact()
    {
        boxCol.enabled = false;
        outline.enabled = false;
        anim.SetTrigger("Interact");
        lineRenderer1.enabled = true;
        lineRenderer2.enabled = true;
        wl1.enabled = true;
        wl2.enabled = true;

        UIManager.Instance.ChangeCursor(false);
    }
}
