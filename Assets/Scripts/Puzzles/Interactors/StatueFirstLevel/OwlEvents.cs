using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OwlEvents : MonoBehaviour
{
    [SerializeField] StatueManager manager;
    public Action<OwlEvents, int> AnimFinishAction;
    [SerializeField] GameObject path;
    [SerializeField] List<GameObject> nodes;
    [SerializeField] List<Tracker> tracker;

    private bool firstInteract = true;

    public void ColliderCallback()
    {
        manager.SetCollider();

        if (firstInteract)
        {
            path.SetActive(true);
            foreach (var node in nodes)
            {
                node.SetActive(true);
            }

            foreach (var tracker in tracker)
            {
                tracker.CanStart = true;
            }

            firstInteract = false;
        }
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