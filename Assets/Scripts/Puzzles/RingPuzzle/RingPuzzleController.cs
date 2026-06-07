using System;
using System.Collections;
using UnityEngine;

public class RingPuzzleController : MonoBehaviour, Interactor
{
    Outline outline;
    BoxCollider coll;
    bool onPuzzle;

    [SerializeField] GameObject puzzleCam;

    private void Update()
    {
        if (!onPuzzle) return;

        if (Input.GetMouseButtonDown(1))
        {
            StartCoroutine(ExitPuzzle());
        }
    }

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
        RingPuzzleManager.Instance.canInteract = false;
        yield return new WaitForSeconds(1.5f);
        RingPuzzleManager.Instance.canInteract = true;
        EventManager.Instance.Dispatch(GameEventTypes.OnPuzzle, this, EventArgs.Empty);
    }

    public IEnumerator ExitPuzzle(bool enableCollider = true)
    {
        RingPuzzleManager.Instance.DeselectAll();
        puzzleCam.SetActive(false);
        RingPuzzleManager.Instance.canInteract = false;

        yield return new WaitForSeconds(1.5f);

        coll.enabled = enableCollider;
        onPuzzle = false;

        EventManager.Instance.Dispatch(GameEventTypes.OnGameplay, this, EventArgs.Empty);
    }
}
