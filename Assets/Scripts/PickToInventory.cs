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
    Heart,
    None
}

public class PickToInventory : MonoBehaviour, Interactor
{
    Outline outline;
    Color originalColor;
    [SerializeField] private ObjectsToPick obj;

    [HideInInspector]
    public bool isOnScale;

    [HideInInspector]
    public Plate plateSide;

    private ObjectType type;

    private void Awake()
    {
        outline = GetComponent<Outline>();
        type = gameObject.GetComponent<ObjectType>();
    }

    private void Start()
    {
        outline.enabled = false;
        originalColor = outline.OutlineColor;
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
        HandInventory.Instance.ShowObjectInHand(obj);
        //Deshabilitamos el objeto pickeado
        gameObject.SetActive(false);
        //Sonido de pickeo
        AudioManager.Instance.PlaySound("Grab");

        if (isOnScale && plateSide != Plate.None)
        {
            WeightManager.Instance.ResultMethod(null, plateSide, type.weight, false);
        }
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
        outline.OutlineColor = originalColor;
        DisableOutline();
    }
}
