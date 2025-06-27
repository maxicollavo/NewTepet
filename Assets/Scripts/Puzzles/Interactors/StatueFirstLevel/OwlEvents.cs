using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OwlEvents : MonoBehaviour
{
    [SerializeField] StatueManager manager;
    [SerializeField] HieroglyficManager hieroManager;
    [SerializeField] CatOnWin catOnWin;
    public Action<OwlEvents, int> AnimFinishAction;
    [SerializeField] GameObject path;
    [SerializeField] List<GameObject> nodes;
    [SerializeField] List<Tracker> tracker;
    [SerializeField] bool isFirstWinPos;

    [SerializeField] int winningPosition;
    private int currentPosition = -1;

    private bool firstInteract = true;

    public bool IsInWinningPosition()
    {
        return currentPosition == winningPosition;
    }

    public void ColliderCallback()
    {
        if (firstInteract)
        {
            if (isFirstWinPos)
            {
                currentPosition = 0;
                AnimFinishAction?.Invoke(this, currentPosition);
            }

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

        if (hieroManager != null)
            if (hieroManager.hasWonPuzzle) return;

        if (catOnWin != null)
            if (catOnWin.HasWon) return;

        Debug.Log("activa collider");
        manager.SetCollider();
    }

    public void FirstCallback()
    {
        Debug.Log("Se llama al primer callback");
        currentPosition = 0;
        AnimFinishAction?.Invoke(this, currentPosition);
    }

    public void SecondCallback()
    {
        Debug.Log("Se llama al segundo callback");
        currentPosition = 1;
        AnimFinishAction?.Invoke(this, currentPosition);
    }
}