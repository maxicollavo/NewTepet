using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnterLevelThree : MonoBehaviour, IEnter
{
    [SerializeField] Animator anim;

    public void Enter()
    {
        ObjectCreator.Instance.canPick = true;
        anim.SetTrigger("Close");
    }
}
