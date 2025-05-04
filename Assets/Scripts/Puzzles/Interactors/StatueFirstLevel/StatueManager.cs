using System;
using UnityEngine;
using UnityEngine.Playables;

public class StatueManager : MonoBehaviour
{
    [SerializeField] StatueInteractor statueInteractor;

    [SerializeField] GameObject blueLight;
    [SerializeField] MeshRenderer headMesh;
    [SerializeField] PlayableDirector timeline;

    public Action<StatueManager> StatueManagerAction;

    private void Start()
    {
        statueInteractor.InteractorAction += OnStatueInteract;
    }

    private void OnStatueInteract(StatueInteractor interactor)
    {
        StatueInteract();
    }

    private void StatueInteract()
    {
        timeline.Play();
        SendActionToJeroglific();
    }

    void SendActionToJeroglific()
    {
        StatueManagerAction?.Invoke(this);
    }
}