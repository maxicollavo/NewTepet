using System.Collections.Generic;
using UnityEngine;

public class LockClick : MonoBehaviour
{
    [SerializeField] private List<GameObject> lockStates;
    private int currentIndex = 0;
    private LockSystem lockSystem;

    private void Awake()
    {
        lockSystem = GetComponent<LockSystem>();

        for (int i = 0; i < lockStates.Count; i++)
        {
            lockStates[i].SetActive(i == currentIndex);
        }
    }

    private void OnMouseDown()
    {
        ChangeState();
        UpdateLockStatus();
    }

    private void UpdateLockStatus()
    {
        lockSystem.SendLock();
    }

    private void ChangeState()
    {
        if (lockStates.Count == 0) return;

        lockStates[currentIndex].SetActive(false);

        currentIndex = (currentIndex + 1) % lockStates.Count;

        lockStates[currentIndex].SetActive(true);
    }
}
