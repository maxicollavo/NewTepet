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
    [SerializeField] GameObject basePyramid;
    GameObject[] basePyramidChildrens;
    [SerializeField] AudioSource door3DSound;


    private void Start()
    {
        pickManager.OnPicking += OnPyramidPicking;

        Transform[] children = basePyramid.GetComponentsInChildren<Transform>(true);
        var childList = new System.Collections.Generic.List<GameObject>();

        foreach (Transform t in children)
        {
            if (t.gameObject != basePyramid)
                childList.Add(t.gameObject);
        }

        basePyramidChildrens = childList.ToArray();
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

    public void PlayDoorSound()
    {
        //AudioManager.Instance.PlaySound("rocaMoviendose");
        door3DSound.Play();
        
    }

    public void ChangeBasePyramidLayer()
    {
        int pyramidLayer = LayerMask.NameToLayer("Pyramid");

        basePyramid.layer = pyramidLayer;

        foreach (GameObject child in basePyramidChildrens)
        {
            child.layer = pyramidLayer;
        }
    }
}
