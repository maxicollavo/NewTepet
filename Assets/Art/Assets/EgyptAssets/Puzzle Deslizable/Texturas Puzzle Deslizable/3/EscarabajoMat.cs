using UnityEngine;
[RequireComponent(typeof(Renderer))]
public class EscarabajoMat : MonoBehaviour
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
        if (active)
        {
            bloomValue = Mathf.MoveTowards(bloomValue, bloomTarget, Time.deltaTime * bloomSpeed);
            UpdateBloom();
        }
        else
        {
            bloomValue = Mathf.MoveTowards(bloomValue, bloomNull, Time.deltaTime * bloomSpeed);
            UpdateBloom();
        }
    }
    public void ActiveBloom()
    {
        active = true;
    }
    public void DesactiveBloom()
    {
        active = false;
    }
    private void UpdateBloom()
    {
        rend.GetPropertyBlock(block);
        block.SetFloat("_Intense", bloomValue);
        rend.SetPropertyBlock(block);
    }
}