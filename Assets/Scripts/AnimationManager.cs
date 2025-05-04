using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    [SerializeField] Animator pyramidAnim;
    private Powers currentPower;

    public void ChangeCurrentPower(Powers power)
    {
        if (currentPower == power) return;

        currentPower = power;

        switch (currentPower)
        {
            case Powers.OnRead:
                pyramidAnim.SetInteger("SelectedPower", 0);
                break;
            case Powers.OnTime:
                pyramidAnim.SetInteger("SelectedPower", 1);
                break;
            default:
                break;
        }
    }
}