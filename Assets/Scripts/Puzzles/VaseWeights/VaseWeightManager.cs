using System.Collections.Generic;
using UnityEngine;

public class VaseWeightManager : MonoBehaviour
{
    [HideInInspector] public int actualIndex;

    public static VaseWeightManager Instance;
    public List<Vase> vases;
    public List<HandVases> handVases;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void TryFillVase()
    {
        vases[actualIndex].TryFillVase();
        handVases[actualIndex].TryFillVase();
    }
}

