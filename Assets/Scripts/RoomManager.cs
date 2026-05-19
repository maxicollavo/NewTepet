using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    private int roomCounter;

    [SerializeField] List<GameObject> tutorialObj;
    [SerializeField] List<GameObject> firstLevelObj;
    [SerializeField] List<GameObject> secondLevelObj;

    private void OnEnable()
    {
        //NewEventManager.OnChangeRoom += TriggerChangeRoom;
    }

    private void OnDisable()
    {
        //NewEventManager.OnChangeRoom -= TriggerChangeRoom;
    }

    private void TriggerChangeRoom()
    {
        OnChangeRoom();
    }

    private void OnChangeRoom()
    {
        roomCounter = GameManager.Instance.roomCounter;

        switch (roomCounter)
        {
            case 0:
                foreach (var obj in tutorialObj)
                {
                    //Destroy(obj);
                }
                break;
            case 1:
                foreach (var obj in firstLevelObj)
                {
                    //Destroy(obj);
                }
                break;
            case 2:
                foreach (var obj in secondLevelObj)
                {
                    //Destroy(obj);
                }
                break;
            default:
                break;
        }

        GameManager.Instance.roomCounter++;
    }
}
