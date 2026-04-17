using System;
using System.Collections;
using UnityEngine;

public class EnterRingPuzzle : MonoBehaviour, Interactor
{
    Outline outline;
    BoxCollider coll;

    [SerializeField] GameObject CM_PuzzleCamera;

    private void Awake()
    {
        outline = GetComponent<Outline>();
        coll = GetComponent<BoxCollider>();
    }
    private void Start()
    {
        outline.enabled = false;
    }

    public void Aiming()
    {
        EnableOutline();
        UIManager.Instance.ChangeCursor(true);
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

    public void Interact()
    {
        StartCoroutine(EnterPuzzleCoroutine());
    }

    private IEnumerator EnterPuzzleCoroutine()
    {
        coll.enabled = false;
        TurnPuzzleCamera(true);
        DisableOutline();
        yield return new WaitForSeconds(1.5f);
        EventManager.Instance.Dispatch(GameEventTypes.OnPuzzle, this, EventArgs.Empty);
    }

    private void TurnPuzzleCamera(bool state)
    {
        if (state)
        {
            CM_PuzzleCamera.SetActive(true);
        }
        else
        {
            CM_PuzzleCamera.SetActive(false);
        }
    }
}
