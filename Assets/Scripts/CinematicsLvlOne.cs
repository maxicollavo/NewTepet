using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;

public class CinematicsLvlOne : MonoBehaviour
{
    [Header("Action Receiving")]
    [SerializeField] BoardPuzzleManager boardManager;

    [Header("References")]
    [SerializeField] CinemachineCamera cam;
    private CinemachineBrain brain;
    [SerializeField] Transform lookAtTarget;
    Transform originalLookAt;
    [SerializeField] List<Animator> doorAnims;

    private void Start()
    {
        brain = Camera.main.GetComponent<CinemachineBrain>();

        boardManager.OnWin += OnBoardWin;
    }

    public void OnBoardWin(BoardPuzzleManager manager)
    {
        StartCoroutine(Cinematic());
    }

    private IEnumerator Cinematic()
    {
        yield return WaitForBlendEnd();

        EventManager.Instance.Dispatch(GameEventTypes.OnCinematic, this, EventArgs.Empty);

        originalLookAt = cam.LookAt;
        cam.LookAt = lookAtTarget;
        cam.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);
        foreach (Animator anim in doorAnims)
        {
            anim.SetTrigger("OpenDoor");
        }

        yield return new WaitForSeconds(2f);

        cam.LookAt = originalLookAt;
        cam.gameObject.SetActive(false);

        yield return new WaitForSeconds(1.5f);

        GameManager.Instance.canCheck = true;
        EventManager.Instance.Dispatch(GameEventTypes.OnGameplay, this, EventArgs.Empty);
    }

    private IEnumerator WaitForBlendEnd()
    {
        while (brain.IsBlending)
            yield return null;
    }
}