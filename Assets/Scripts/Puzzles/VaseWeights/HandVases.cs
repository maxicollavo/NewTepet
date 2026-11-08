using UnityEngine;

public class HandVases : MonoBehaviour
{
    public int vaseIndex;

    public void TryFillVase()
    {
        Debug.Log("Llenando el jarrón");
        GetComponentInChildren<VasijaArenaVisual>(true)?.SumarNivel(); // arena visual (Bauti) - borrar si molesta
    }
}
