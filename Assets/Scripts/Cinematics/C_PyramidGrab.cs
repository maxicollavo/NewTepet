using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;

public class C_PyramidGrab : MonoBehaviour
{
    [Header("Action Receiving")]
    [SerializeField] PyramidPicking pickManager;

    [Header("References")]
    [SerializeField] PlayableDirector pickPyramidTimeline;

    private void Start()
    {
        pickManager.OnPicking += OnPyramidPicking;
    }

    private void OnDestroy()
    {
        pickManager.OnPicking -= OnPyramidPicking;
    }

    public void OnPyramidPicking(PyramidPicking manager)
    {
        Cinematic();
    }

    private void Cinematic()
    {
        pickPyramidTimeline.Play();
        AudioManager.Instance.PlaySound("Grab");
        EventManager.Instance.Dispatch(GameEventTypes.OnCinematic, this, EventArgs.Empty);
    }

    public void CallOnGameplay()
    {
        EventManager.Instance.Dispatch(GameEventTypes.OnGameplay, this, EventArgs.Empty);
    }
}
