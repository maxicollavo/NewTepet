using System.Collections;
using UnityEngine;

public class SarcoInteract : MonoBehaviour, Interactor
{
    Outline outline;

    [SerializeField] Animator anim;
    [SerializeField] GameObject canvasBlackScreen;

    public bool canInteract = true;

    private void Awake()
    {
        outline = GetComponent<Outline>();
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

        canvasBlackScreen.SetActive(true);

        yield return new WaitForSeconds(1f);
        FadeBlackImage.Instance.StartFadeToBlack(2f);
        yield return new WaitForSeconds(1f);
        FadeBlackImage.Instance.StartFadeFromBlack(2f);

        Vector3 teleportPosition = new Vector3(180.9959f, -13.98034f, -78.98776f);
        Quaternion teleportRotation = Quaternion.Euler(0f, 538.999f, 0f);

        NewEventManager.TriggerChangeRoom(teleportPosition, teleportRotation);
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