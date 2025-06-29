using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckLevelThree : MonoBehaviour, IEnter
{
    [SerializeField] Animator closeDoorAnim;
    [SerializeField] Animator openDoorAnim;
    [SerializeField] AudioSource doorSound;

    public void Enter()
    {
        closeDoorAnim.SetTrigger("Close");

        if (!HandInventory.hasObjInHand)
        {
            doorSound.Play();
            openDoorAnim.SetTrigger("Open");
        }
    }
}
