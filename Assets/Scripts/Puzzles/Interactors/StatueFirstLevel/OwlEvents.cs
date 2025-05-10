using System;
using UnityEngine;

public class OwlEvents : MonoBehaviour
{
    [SerializeField] StatueManager manager;
    public Action<OwlEvents, int> AnimFinishAction;

    public void ColliderCallback()
    {
        manager.SetCollider();
    }

    public void FirstCallback()
    {
        AnimFinishAction?.Invoke(this, 0);
    }

    public void SecondCallback()
    {
        AnimFinishAction?.Invoke(this, 1);
    }
}