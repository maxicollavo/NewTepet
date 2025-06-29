using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckLevelThree : MonoBehaviour, IEnter
{
    [SerializeField] Animator closeDoorAnim;
    [SerializeField] Animator openDoorAnim;

    public void Enter()
    {
        closeDoorAnim.SetTrigger("Close");

        if (!HandInventory.hasObjInHand)
        {
            openDoorAnim.SetTrigger("Open");
        }
    }
}
