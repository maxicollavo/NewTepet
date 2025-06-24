using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;

public class HieroglyficManager : MonoBehaviour
{
    public Action<HieroglyficManager> OnWinAction;

    private int counter;
    [HideInInspector] public bool hasWonPuzzle;
    [SerializeField] PlayableDirector cinematic;
    [SerializeField] Light[] lights;

    [Header("References")]
    [SerializeField] CinemachineCamera cam;
    private CinemachineBrain brain;
    [SerializeField] Transform lookAtTarget;
    Transform originalLookAt;
    [SerializeField] Light[] owlLights;

    private bool hasEndedCinematic;
    public ParticleSystem rocksParticle;
    public static HieroglyficManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        brain = Camera.main.GetComponent<CinemachineBrain>();
    }

    public void CheckToUpdateCounter()
    {
        if (hasEndedCinematic) return;

        counter++;
        CheckPuzzleWin();
    }

    public void CheckPuzzleWin()
    {
        if (hasEndedCinematic) return;

        if (counter == 2 & OwlManager.Instance.hasWon)
        {
            hasWonPuzzle = true;
            OnWinAction?.Invoke(this);
            OwlManager.Instance.DeactivateColliders();
            StartCoroutine(Cinematic());
        }
    }

    private IEnumerator Cinematic()
    {
        yield return new WaitForSeconds(1.5f);
        EventManager.Instance.Dispatch(GameEventTypes.OnCinematic, this, EventArgs.Empty);
        originalLookAt = cam.LookAt;
        cam.LookAt = lookAtTarget;
        cam.gameObject.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        foreach (var light in lights)
        {
            light.intensity = 5;
        }

        yield return new WaitForSeconds(1.5f);
        cinematic.Play();
        AudioManager.Instance.PlaySound("OpenTramp");
        rocksParticle.Play();
        yield return new WaitForSeconds(0.5f);

        AudioManager.Instance.PlaySound("StoneParticleSound");
        yield return new WaitForSeconds(0.5f);

        AudioManager.Instance.PlaySound("StopBall");
        yield return new WaitForSeconds(2f);
        cam.LookAt = originalLookAt;
        cam.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.75f);
        foreach (var light in lights)
        {
            light.intensity = 30;
        }

        yield return new WaitForSeconds(1.5f);
        hasEndedCinematic = true;
        EventManager.Instance.Dispatch(GameEventTypes.OnGameplay, this, EventArgs.Empty);
    }
}
