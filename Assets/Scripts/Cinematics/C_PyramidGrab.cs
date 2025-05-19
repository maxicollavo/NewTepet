using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class C_PyramidGrab : MonoBehaviour
{
    [Header("Action Receiving")]
    [SerializeField] PyramidPicking pickManager;

    [Header("References")]
    [SerializeField] CinemachineCamera lookAtDoorCam;
    [SerializeField] CinemachineCamera lookAtStandCam;
    private CinemachineBrain brain;
    [SerializeField] Transform lookAtTarget;
    Transform originalLookAt;
    [SerializeField] Animator doorAnim;

    private void Start()
    {
        brain = Camera.main.GetComponent<CinemachineBrain>();
        pickManager.OnPicking += OnPyramidPicking;
    }

    private void OnDestroy()
    {
        pickManager.OnPicking -= OnPyramidPicking;
    }

    public void OnPyramidPicking(PyramidPicking manager)
    {
        StartCoroutine(Cinematic());
    }

    private IEnumerator Cinematic()
    {
        lookAtStandCam.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);

        EventManager.Instance.Dispatch(GameEventTypes.OnCinematic, this, EventArgs.Empty);
        lookAtDoorCam.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.3f);

        doorAnim.SetTrigger("OpenDoor");
        yield return new WaitForSeconds(1f);

        lookAtDoorCam.gameObject.SetActive(false);
        yield return new WaitForSeconds(2f);

        lookAtStandCam.gameObject.SetActive(false);
        EventManager.Instance.Dispatch(GameEventTypes.OnGameplay, this, EventArgs.Empty);
    }

    private IEnumerator WaitForBlendEnd()
    {
        while (brain.IsBlending)
            yield return null;
    }
}
