using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OwlManager : MonoBehaviour
{
    [SerializeField] List<OwlEvents> owlEvents;
    Dictionary<OwlEvents, bool> owlStates = new Dictionary<OwlEvents, bool>();
    public bool hasWon;
    public List<BoxCollider> colliders;

    public static OwlManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        foreach (var owlEvent in owlEvents)
        {
            owlEvent.AnimFinishAction += OnAnimFinish;
            owlStates[owlEvent] = false;
        }
    }

    private void OnAnimFinish(OwlEvents events, int arg2)
    {
        bool allOwlsCorrect = true;
        foreach (var owl in owlEvents)
        {
            if (!owl.IsInWinningPosition())
            {
                allOwlsCorrect = false;
                break;
            }
        }

        hasWon = allOwlsCorrect;

        if (hasWon)
        {
            HieroglyficManager.Instance.CheckPuzzleWin();
        }
    }

    public void DeactivateColliders()
    {
        foreach (var coll in colliders)
        {
            Debug.Log("Desactiva los colliders de los buhos");
            coll.enabled = false;
        }
    }
}
