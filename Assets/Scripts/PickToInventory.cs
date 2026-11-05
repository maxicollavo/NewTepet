using System.Collections;
using UnityEngine;

public enum ObjectsToPick
{
    BoardPiece,
    GlassSphere,
    ColumnSphere,
    Feather,
    Stone,
    Knife,
    Canopo,
    Djed,
    Heart,
    Vase0,
    Vase1,
    Vase2,
    Vase3,
    None
}

public class PickToInventory : MonoBehaviour, Interactor
{
    Outline outline;
    Color originalColor;
    [SerializeField] private ObjectsToPick obj;

    [HideInInspector]
    public bool isOnScale;

    public bool canBePicked;

    [HideInInspector]
    public Plate plateSide;

    private ObjectType type;

    [SerializeField] private bool _disablesIntroLight;
    [SerializeField] private GameObject _introLight;

    private void Awake()
    {
        outline = GetComponent<Outline>();
        type = gameObject.GetComponent<ObjectType>();
    }

    private void Start()
    {
        outline.enabled = false;
        originalColor = outline.OutlineColor;
        canBePicked = true;
    }

    private void Update()
    {
        if (!ObjectCreator.Instance.canPick)
        {
            canBePicked = false;
        }
    }

    [SerializeField] private bool isVase;

    public void Interact()
    {
        if (isVase)
        {
            Vase vase = gameObject.GetComponent<Vase>();
            VaseWeightManager.Instance.actualIndex = vase.vaseIndex;
        }

        if (!canBePicked) return;

        if (HandInventory.IsHoldingSomething())
        {
            StartCoroutine(CannotPick());
            return;
        }

        ObjectCreator.Instance.RemoveSpawnedObject(gameObject);
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
        //Si necesita hacer una acci�n al pickear

        if (_disablesIntroLight)
        {
            if (_introLight == null) return;
            _introLight.SetActive(false);
        }

        //if (isOnScale && plateSide != Plate.None)
        //{
        //    WeightManager.Instance.ResultMethod(plateSide, type.weight, false, false);
        //}
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
        if (!canBePicked) return;

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
