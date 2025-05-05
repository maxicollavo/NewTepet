using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;

public class HieroglyficManager : MonoBehaviour
{
    public int counter;
    [SerializeField] PlayableDirector cinematic;

    [Header("References")]
    [SerializeField] CinemachineCamera cam;
    private CinemachineBrain brain;
    [SerializeField] Transform lookAtTarget;
    Transform originalLookAt;

    private void Start()
    {
        brain = Camera.main.GetComponent<CinemachineBrain>();
    }

    public void CanOpenCeiling()
    {
        counter++;

        if (counter == 4)
        {
            StartCoroutine(Cinematic());
        }
    }

    private IEnumerator Cinematic()
    {
        yield return WaitForBlendEnd();

        EventManager.Instance.Dispatch(GameEventTypes.OnCinematic, this, EventArgs.Empty);

        originalLookAt = cam.LookAt;
        cam.LookAt = lookAtTarget;
        cam.gameObject.SetActive(true);

        yield return new WaitForSeconds(2f);
        cinematic.Play();

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
