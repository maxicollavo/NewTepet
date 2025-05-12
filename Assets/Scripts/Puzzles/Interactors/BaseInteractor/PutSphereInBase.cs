using System.Collections;
using UnityEngine;

public class PutSphereInBase : MonoBehaviour, Interactor
{
    Outline outline;

    [SerializeField] private GameObject sphereInHand;
    [SerializeField] private GameObject baseSphere;
    private bool canUse = true;

    private void Awake()
    {
        outline = GetComponent<Outline>();
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
        if (!canUse) return;

        EnableOutline();
        UIManager.Instance.ChangeCursor(true);
    }

    public void Interact()
    {
        if (sphereInHand.activeInHierarchy)
        {
            DisableOutline();
            sphereInHand.SetActive(false);
            baseSphere.SetActive(true);
            HandInventory.hasObjInHand = false;
            canUse = false;
            this.enabled = false;
        }
        else
        {
            StartCoroutine(CannotPick());
        }
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
