using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class C_PyramidGrab : MonoBehaviour
{
    [Header("Action Receiving")]
    [SerializeField] PyramidPicking pickManager;

    [Header("References")]
    [SerializeField] CinemachineCamera cam;
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
        yield return WaitForBlendEnd();

        EventManager.Instance.Dispatch(GameEventTypes.OnCinematic, this, EventArgs.Empty);

        originalLookAt = cam.LookAt;
        cam.LookAt = lookAtTarget;
        cam.gameObject.SetActive(true);

        yield return new WaitForSeconds(1f);
        doorAnim.SetTrigger("OpenDoor");

        yield return new WaitForSeconds(1f);

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
