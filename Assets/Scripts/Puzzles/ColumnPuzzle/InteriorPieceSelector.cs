using UnityEngine;

[RequireComponent(typeof(Outline))]
public class InteriorPieceSelector : MonoBehaviour
{
    Outline outline;
    public Transform columnTransform;

    private void Awake()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false;
    }

    public void EnableOutline()
    {
        outline.enabled = true;
    }

    public void DisableOutline()
    {
        outline.enabled = false;
    }
}
