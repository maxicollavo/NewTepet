using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckLevelThree : MonoBehaviour, IEnter
{
    [SerializeField] Animator closeDoorAnim;
    [SerializeField] Animator openDoorAnim;
    [SerializeField] AudioSource doorSound;

    bool hasBeenActivated;

    public void Enter()
    {
        if (hasBeenActivated) return;

        closeDoorAnim.SetTrigger("Close");

        if (!HandInventory.hasObjInHand)
        {
            doorSound.Play();
            openDoorAnim.SetTrigger("Open");
            EventManager.Instance.Dispatch(GameEventTypes.OnDoorOpen, this, EventArgs.Empty);
        }

        hasBeenActivated = true;
    }
}
