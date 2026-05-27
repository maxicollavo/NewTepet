using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class PutSphereInBase : MonoBehaviour, Interactor
{
    Outline outline;
    BoxCollider coll;

    [SerializeField] ObjectsToPick requiredObj;

    [SerializeField] PlayableDirector sphereTravelToBase;
    [SerializeField] GameObject CM_PuzzleCamera;

    [SerializeField] ParabolaController parabolaController;

    private void Awake()
    {
        outline = GetComponent<Outline>();
        coll = GetComponent<BoxCollider>();
    }

    private void Start()
    {
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
        if (PickedObjData.Instance.WasPicked(requiredObj))
        {
            StartCoroutine(EnterPuzzleCoroutine());
        }
        else
        {
            StartCoroutine(CannotPick());
        }
    }

    [SerializeField] RotateSphere sphere;

    public IEnumerator EnterPuzzleCoroutine()
    {
        parabolaController.FollowParabolaTo(this.transform);
        //PickedObjData.Instance.MarkAsThrowed(requiredObj, false); CUANDO TODO ME FUNCIONA DESCOMENTAR
        //coll.enabled = false; CUANDO TODO ME FUNCIONA DESCOMENTAR
        DisableOutline();
        //Encendemos la esfera de puzzle
        //TurnPuzzleCamera(true);
        yield return new WaitForSeconds(1f);
        //sphere.gameObject.SetActive(true); CUANDO TODO ME FUNCIONA DESCOMENTAR
        //sphere.onBase = true;
        //yield return new WaitForSeconds(1.5f);
        //sphereTravelToBase.Play();
        //EventManager.Instance.Dispatch(GameEventTypes.OnPuzzle, this, EventArgs.Empty);
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

    private IEnumerator CannotPick()
    {
        EnableOutline();
        var originalColor = outline.OutlineColor;
        outline.OutlineColor = Color.red;
        yield return new WaitForSeconds(0.5f);
        outline.OutlineColor = originalColor;
        DisableOutline();
    }
}
