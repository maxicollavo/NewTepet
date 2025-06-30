using System.Collections;
using UnityEngine;

public class SarcoInteract : MonoBehaviour, Interactor
{
    Outline outline;
    BoxCollider coll;

    [SerializeField] Animator anim;

    [HideInInspector] public bool canInteract;

    private void Awake()
    {
        outline = GetComponent<Outline>();
        coll = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        outline.enabled = false;
    }

    public void Interact()
    {
        if (!canInteract) return;

        StartCoroutine(OpenSarco());
    }

    public IEnumerator OpenSarco()
    {
        anim.SetTrigger("Open");
        yield return new WaitForSeconds(1f);
        //Hacer que cambie la pantalla a negro o te lleve al menu de terminar demo
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
        if (!canInteract) return;

        EnableOutline();
        UIManager.Instance.ChangeCursor(true);
    }
}
