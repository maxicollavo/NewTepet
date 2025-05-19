using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class PieceBoxOnWin : MonoBehaviour
{
    [Header("References")]
    [SerializeField] TrackerManager manager;
    [SerializeField] CinemachineCamera cinematicCam;
    [SerializeField] BoxCollider interactorColl;
    [SerializeField] BoxCollider pieceColl;
    private CinemachineBrain brain;
    [SerializeField] Animator boxAnim;
    [SerializeField] Transform lookAtTarget;
    Transform originalLookAt;

    void Start()
    {
        manager.JeroglificAction += Win;

        brain = Camera.main.GetComponent<CinemachineBrain>();
    }

    void Win(TrackerManager manager)
    {
        StartCoroutine(Cinematic());
    }

    private IEnumerator Cinematic()
    {
        interactorColl.enabled = false;
        cinematicCam.gameObject.SetActive(true);
        EventManager.Instance.Dispatch(GameEventTypes.OnCinematic, this, EventArgs.Empty);
        yield return new WaitForSeconds(1.3f);
        interactorColl.enabled = false;

        boxAnim.SetTrigger("Open");
        interactorColl.enabled = false;
        yield return new WaitForSeconds(1f);

        interactorColl.enabled = false;
        cinematicCam.gameObject.SetActive(false);
        pieceColl.enabled = true;
        EventManager.Instance.Dispatch(GameEventTypes.OnGameplay, this, EventArgs.Empty);
    }
}