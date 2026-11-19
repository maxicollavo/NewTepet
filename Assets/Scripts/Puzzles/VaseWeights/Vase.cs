using UnityEngine;

public class Vase : MonoBehaviour
{
    public int vaseIndex;

    public void TryFillVase()
    {
        Debug.Log("Llenando el jarrón");
        GetComponentInChildren<VasijaArenaVisual>(true)?.SumarNivel(); // arena
    }
}