using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class PieceBoxOnWin : MonoBehaviour
{
    [Header("References")]
    [SerializeField] TrackerManager manager;
    [SerializeField] CinemachineCamera cam;
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
        yield return WaitForBlendEnd();

        EventManager.Instance.Dispatch(GameEventTypes.OnCinematic, this, EventArgs.Empty);

        originalLookAt = cam.LookAt;
        cam.LookAt = lookAtTarget;
        cam.gameObject.SetActive(true);

        yield return new WaitForSeconds(1f);

        boxAnim.SetTrigger("Open");

        yield return new WaitForSeconds(1f);

        cam.LookAt = originalLookAt;
        cam.gameObject.SetActive(false);

        yield return new WaitForSeconds(1.5f);

        pieceColl.enabled = true;

        EventManager.Instance.Dispatch(GameEventTypes.OnGameplay, this, EventArgs.Empty);
    }

    private IEnumerator WaitForBlendEnd()
    {
        while (brain.IsBlending)
            yield return null;
    }
}