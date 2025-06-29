using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;

public class LevelThreeHieroglyficsOnWin : MonoBehaviour
{
    public Action OnWinAction;

    private int counter;
    [HideInInspector] public bool hasWonPuzzle;
    [SerializeField] Animator[] anims;
    private Transform lerpLookAt;

    [Header("References")]
    [SerializeField] CinemachineCamera cam;
    private CinemachineBrain brain;
    public Renderer[] targetRenderers;
    [Header("La Trampilla, hacia donde va a mirar el jugador")]
    [SerializeField] Transform[] lookAtTargets;
    Transform originalLookAt;
    [SerializeField] OwlManager owlManager;

    private bool hasEndedCinematic;
    //public ParticleSystem rocksParticle;
    //public GameObject RockGO;


    private void Start()
    {
        brain = Camera.main.GetComponent<CinemachineBrain>();
    }

    public void CheckToUpdateCounter()
    {
        if (hasEndedCinematic) return;

        counter++;
        Debug.Log($"El contador está en {counter}");
        CheckPuzzleWin();
    }

    public void CheckPuzzleWin()
    {
        if (hasEndedCinematic) return;

        if (counter == 2)
        {
            Debug.Log($"Se ganaron los 2 jeroglificos");
            hasWonPuzzle = true;
            owlManager.DeactivateColliders();
            StartCoroutine(Cinematic());
        }
    }

    private IEnumerator Cinematic()
    {
        yield return new WaitForSeconds(1.5f);
        EventManager.Instance.Dispatch(GameEventTypes.OnCinematic, this, EventArgs.Empty);

        originalLookAt = cam.LookAt;

        // Creamos dummy para interpolar
        if (lerpLookAt == null)
        {
            GameObject go = new GameObject("LerpLookAt");
            lerpLookAt = go.transform;
        }

        // Posicionamos el dummy en el primer target
        lerpLookAt.position = lookAtTargets[0].position;
        cam.LookAt = lerpLookAt;
        cam.gameObject.SetActive(true);

        yield return new WaitForSeconds(1.5f);
        foreach (var anim in anims)
            anim.SetTrigger("Open");

        yield return new WaitForSeconds(1f);

        // Interpolamos hacia el segundo target
        yield return StartCoroutine(LerpLookAtTarget(lookAtTargets[1].position, 1f)); // duracion 1 segundo

        foreach (var item in targetRenderers)
        {
            Material[] mats = item.materials;

            if (mats.Length > 0)
            {
                mats[0].EnableKeyword("_EMISSION");
                mats[0].SetColor("_EmissionColor", Color.yellow);
                item.materials = mats;
            }
        }

        yield return new WaitForSeconds(2f);

        cam.LookAt = originalLookAt;
        cam.gameObject.SetActive(false);

        yield return new WaitForSeconds(1.5f);
        hasEndedCinematic = true;
        EventManager.Instance.Dispatch(GameEventTypes.OnGameplay, this, EventArgs.Empty);
        OnWinAction?.Invoke();
    }

    private IEnumerator LerpLookAtTarget(Vector3 targetPos, float duration)
    {
        float time = 0f;
        Vector3 start = lerpLookAt.position;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            lerpLookAt.position = Vector3.Lerp(start, targetPos, t);
            yield return null;
        }

        lerpLookAt.position = targetPos;
    }
}
