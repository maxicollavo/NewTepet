using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OwlBoardEvents : MonoBehaviour
{
    [SerializeField] OwlInteraction owlInteraction;

    public void OwlOnRightCallout()
    {
        owlInteraction.OwlOnRight();
    }

    public void OwlOnLeftCallout()
    {
        owlInteraction.OwlOnLeft();
    }

    public void TurnOffLeft()
    {
        owlInteraction.TurnOffLeft();
    }

    public void TurnOffRight()
    {
        owlInteraction.TurnOffRight();
    }
}
