using UnityEngine;

public class LockSystem : MonoBehaviour
{
    private int posCounter;
    private int minCounter = 1;
    private int maxCounter = 3;

    [SerializeField] private int desiredPos;
    [SerializeField] private int lockNum;

    [SerializeField] LockManager lockManager;

    private void Awake()
    {
        posCounter = minCounter;
    }

    private void Start()
    {
        CheckInitialState();
    }

    public void SendLock()
    {
        posCounter++;

        if (posCounter > maxCounter)
        {
            posCounter = minCounter;
        }

        UpdateLockState();
    }

    private void CheckInitialState()
    {
        UpdateLockState();
    }

    private void UpdateLockState()
    {
        if (posCounter == desiredPos)
        {
            lockManager.LockDone[lockNum] = true;
        }
        else
        {
            lockManager.LockDone[lockNum] = false;
        }

        lockManager.CheckLock();
    }
}