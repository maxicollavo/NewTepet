using UnityEngine;
using UnityEngine.VFX;


[RequireComponent(typeof(Outline))]
public class InteriorPieceSelector : MonoBehaviour
{
    Outline outline;
    BoxCollider coll;
    public Transform columnTransform;
    [SerializeField] ColumnInteractManager interactManager;

    public Transform forward;
    public Transform lookAtTarget;
    [HideInInspector] public bool isAligned;
    [HideInInspector] public bool hasWon;

    public VisualEffect[] vfxEffects;

    private void Awake()
    {
        outline = GetComponent<Outline>();
        coll = GetComponent<BoxCollider>();
        outline.enabled = false;

        if (!interactManager.interiorPieceSelected.ContainsKey(this))
        {
            interactManager.interiorPieceSelected.Add(this, false);
        }
    }

    public void EnableOutline()
    {
        outline.enabled = true;
    }

    public void DisableOutline()
    {
        outline.enabled = false;
    }

    public void OnWin()
    {
        hasWon = true;
        outline.enabled = false;
        coll.enabled = false;
    }
}
