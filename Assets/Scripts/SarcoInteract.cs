using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        canInteract = false;
        DisableOutline();
        anim.SetTrigger("Open");
        yield return new WaitForSeconds(1f);
        FadeBlackImage.Instance.StartFadeIn(2f);
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Endgame");
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
