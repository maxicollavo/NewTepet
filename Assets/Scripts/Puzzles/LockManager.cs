using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class LockManager : MonoBehaviour
{
    [HideInInspector] public List<bool> LockDone = new List<bool>();
    [HideInInspector] public bool HasWon;

    [SerializeField]
    private int maxLocks;

    [SerializeField]
    private GameObject interactTrigger;

    [SerializeField] AnimacionesPyramid pyramid;

    [SerializeField] GameObject playerCam;
    [SerializeField] GameObject lockCam;

    public static LockManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            InitializeLocks(maxLocks);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void InitializeLocks(int count)
    {
        LockDone = new List<bool>();

        for (int i = 0; i < count; i++)
        {
            LockDone.Add(false);
        }
    }

    public void CheckLock()
    {
        if (HasWon) return;

        int solved = 0;
        for (int i = 0; i < LockDone.Count; i++)
        {
            if (LockDone[i])
            {
                solved++;
            }
        }

        if (solved == LockDone.Count)
        {
            StartCoroutine(Win());
        }
    }

    private void BackToGameplayCamera()
    {
        playerCam.SetActive(true);
        lockCam.SetActive(false);
    }

    private IEnumerator Win()
    {
        EventManager.Instance.Dispatch(GameEventTypes.OnGameplay, this, EventArgs.Empty);
        BackToGameplayCamera();
        HasWon = true;

        Destroy(interactTrigger);

        yield return new WaitForSeconds(0.1f);
        pyramid.RestartAnim();
    }
}
