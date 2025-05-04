using System.Collections.Generic;
using UnityEngine;

public class PowerManager : MonoBehaviour
{
    [SerializeField] List<GameObject> powersUI;
    [SerializeField] Detection detection;

    private void Start()
    {
        detection.ChangePowerAction += OnChangePower;
    }

    void OnChangePower(Powers pow)
    {
        foreach (var ui in powersUI)
            ui.SetActive(false);

        int index = (int)pow;
        if (index >= 0 && index < powersUI.Count)
        {
            powersUI[index].SetActive(true);
        }
        else
        {
            Debug.LogWarning($"No UI assigned for power {pow}");
        }
    }
}
