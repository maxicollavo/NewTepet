using System.Collections;
using UnityEngine;

public enum ObjectsToPick
{
    BoardPiece,
    GlassSphere,
    Feather,
    Stone,
    Knife,
    Canopo,
    Djed,
    Heart
}

public class PickToInventory : MonoBehaviour, Interactor
{
    Outline outline;
    [SerializeField] private ObjectsToPick obj;

    [SerializeField] HandInventory handInventory;

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
        if (HandInventory.IsHoldingSomething())
        {
            StartCoroutine(CannotPick());
            return;
        }

        //Marcamos que pickeamos un objeto y cual es
        PickedObjData.Instance.MarkAsPicked(obj);
        //Deshabilitamos el Outline
        DisableOutline();
        //Encendemos el objeto de la mano
        if (handInventory != null)
            handInventory.ShowObjectInHand(obj);
        //Deshabilitamos el objeto pickeado
        gameObject.SetActive(false);
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

    private IEnumerator CannotPick()
    {
        EnableOutline();
        outline.OutlineColor = Color.red;
        yield return new WaitForSeconds(0.5f);
        outline.OutlineColor = Color.white;
        DisableOutline();
    }
}
