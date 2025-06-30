using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterLevelThree : MonoBehaviour, IEnter
{
    [SerializeField] Animator anim;
    [SerializeField] AudioSource doorSound;

    bool state;

    public void Enter()
    {
        if (state) return;

        ObjectCreator.Instance.canPick = true;
        anim.SetTrigger("Close");
        doorSound.Play();
        state = true;
    }
}
