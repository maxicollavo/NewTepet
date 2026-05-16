using System.Collections;
using UnityEngine;

public class RingPuzzleController : MonoBehaviour, Interactor
{
    Outline outline;
    BoxCollider coll;

    [SerializeField] GameObject puzzleCam;
    bool onPuzzle;
    bool canRotateRings;

    private void Start()
    {
        outline = GetComponent<Outline>();
        coll = GetComponent<BoxCollider>();
        outline.enabled = false;
    }

    public void Interact()
    {
        if (onPuzzle) return;

        StartCoroutine(EnterPuzzle());
    }

    public void DisableOutline()
    {
        outline.enabled = false;

        UIManager.Instance.ChangeCursor(false);
    }

    public void EnableOutline()
    {
        outline.enabled = true;
    }

    public void Aiming()
    {
        if (onPuzzle) return;

        EnableOutline();
        UIManager.Instance.ChangeCursor(true);
    }

    public IEnumerator EnterPuzzle()
    {
        DisableOutline();
        coll.enabled = false;
        onPuzzle = true;
        puzzleCam.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        canRotateRings = true;
    }

    private IEnumerator ExitPuzzle()
    {
        canRotateRings = false;
        puzzleCam.SetActive(false);
        yield return new WaitForSeconds(1.5f);
        coll.enabled = true;
        onPuzzle = false;
    }
}
