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

        if (isOnScale)
        {
            if (plateSide == Plate.Left)
            {
                WeightManager.Instance.leftWeight -= type.weight;
                Debug.Log($"El weight izquierdo es de {WeightManager.Instance.leftWeight}");
            }
            else
            {
                WeightManager.Instance.rightWeight -= type.weight;
                Debug.Log($"El weight derecho es de {WeightManager.Instance.rightWeight}");
            }
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
        var originalColor = outline.OutlineColor;
        outline.OutlineColor = Color.red;
        yield return new WaitForSeconds(0.5f);
        outline.OutlineColor = originalColor;
        DisableOutline();
    }
}
