using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class BloomController : MonoBehaviour
{
    private Renderer rend;
    private MaterialPropertyBlock block;

    public float bloomValue = 0f;

    [Header("Bloom setting")]
    public int bloomTarget = 37;
    public int bloomNull = 0;
    public int bloomSpeed = 20;

    private bool active = false;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        block = new MaterialPropertyBlock();
    }

    private void Update()
    {
        float target = active ? bloomTarget : bloomNull;

        bloomValue = Mathf.MoveTowards(
            bloomValue,
            target,
            Time.deltaTime * bloomSpeed
        );

        UpdateBloom();
    }

    public void ActiveBloom()
    {
        active = true;
        Debug.Log("Bloom activado");
    }

    public void DesactiveBloom()
    {
        active = false;
        Debug.Log("Bloom desactivado");
    }

    private void UpdateBloom()
    {
        rend.GetPropertyBlock(block);
        block.SetFloat("_Intense", bloomValue);
        rend.SetPropertyBlock(block);
    }
}