using System.Collections;
using UnityEngine;

public enum ObjectsToPick
{
    BoardPiece,
    GlassSphere
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
        if (HandInventory.hasObjInHand)
        {
            StartCoroutine(CannotPick());
            return;
        }

        DisableOutline();
        HandInventory.hasObjInHand = true;
        if (handInventory != null)
        {
            handInventory.ShowObjectInHand(obj);
        }

        gameObject.SetActive(false);

        if (obj == ObjectsToPick.BoardPiece)
        {
            GameManager.Instance.HasPiece = true;
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
        outline.OutlineColor = Color.white;
        DisableOutline();
    }
}
