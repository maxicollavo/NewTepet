using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;

public class TrackerManager : MonoBehaviour
{
    [HideInInspector] public List<Tracker> trackerList = new List<Tracker>();
    public Action<TrackerManager> HieroglyphCompletedAction;
    public FollowMouseClick trail;
    [SerializeField] HieroglyficArmHandler armHandler;
    [SerializeField] Transform rightArm;
    [SerializeField] GameObject interactor;
    BoxCollider interactorCollider;
    PuzzleInteractor puzzleInteractor;
    public bool OnPuzzle { get; private set; }
    public bool HasWon;
    bool canGoBack;
    public bool canInteract;
    public bool subFloor;
    public bool isTarget;
    [SerializeField] GameObject CM_PuzzleCamera;
    [SerializeField] HieroglyficManager hieroglyficManager;
    [SerializeField] Transform[] armsTransforms;
    // Diccionario para guardar las rotaciones originales de las manos originales
    private Dictionary<Transform, Quaternion> originalRotations = new Dictionary<Transform, Quaternion>();

    private void Awake()
    {
        puzzleInteractor = interactor.GetComponent<PuzzleInteractor>();
        interactorCollider = interactor.GetComponent<BoxCollider>();
    }

    private void Start()
    {
        puzzleInteractor.PuzzleAction += OnPuzzleMethod;

        // Guardamos la rotación original de cada brazo
        foreach (var arm in armsTransforms)
        {
            if (arm != null)
                originalRotations[arm] = arm.localRotation;
        }

        if (hieroglyficManager == null) return;
        hieroglyficManager.OnWinAction += OnWinMethod;
    }

    private void OnWinMethod(HieroglyficManager manager)
    {
        if (!interactorCollider.enabled) return;
        interactorCollider.enabled = false;
    }

    private void Update()
    {
        if (!canGoBack) return;

        if (Input.GetKeyDown(KeyCode.Mouse1) && OnPuzzle)
        {
            BackToGameplay(false);
        }
    }

    public void TurnPuzzleCamera(bool state)
    {
        CM_PuzzleCamera.SetActive(state);
    }

    private void OnPuzzleMethod(PuzzleInteractor interactor)
    {
        if (HasWon) return;

        OnPuzzle = true;

        foreach (var arm in armsTransforms)
        {
            StartCoroutine(RotateArmToX(arm, 90f, 0.5f));
        }

        TurnPuzzleCamera(true);
        interactor.DisableOutline();
        interactorCollider.enabled = false;
        StartCoroutine(EnterPuzzleCoroutine());
    }

    public IEnumerator EnterPuzzleCoroutine()
    {
        canGoBack = false;
        canInteract = false;
        yield return new WaitForSeconds(1.3f);
        armHandler.EnableArm(rightArm);
        yield return new WaitForSeconds(.5f);
        canGoBack = true;
        canInteract = true;
        EventManager.Instance.Dispatch(GameEventTypes.OnPuzzle, this, EventArgs.Empty);
    }

    public void BackToGameplay(bool onWin)
    {
        if (HasWon) return;

        OnPuzzle = false;
        armHandler.DisableArm(rightArm);
        trail.gameObject.SetActive(false);
        EventManager.Instance.Dispatch(GameEventTypes.OnGameplay, this, EventArgs.Empty);
        StartCoroutine(ExitPuzzleCoroutine(onWin));
    }

    public IEnumerator ExitPuzzleCoroutine(bool hasWon)
    {
        yield return new WaitForSeconds(0.3f);
        TurnPuzzleCamera(false);
        canGoBack = false;
        yield return new WaitForSeconds(1f);
        foreach (var arm in armsTransforms)
        {
            Quaternion original = originalRotations[arm];
            StartCoroutine(RotateToRotation(arm, original, 0.5f));
        }
        yield return new WaitForSeconds(.5f);
        canGoBack = true;

        if (!hasWon)
            interactorCollider.enabled = true;
    }

    private IEnumerator RotateArmToX(Transform arm, float targetX, float duration)
    {
        Quaternion startRotation = arm.localRotation;

        Vector3 currentEuler = arm.localEulerAngles;
        Vector3 targetEuler = new Vector3(targetX, currentEuler.y, currentEuler.z);
        Quaternion targetRotation = Quaternion.Euler(targetEuler);

        float time = 0f;
        while (time < duration)
        {
            arm.localRotation = Quaternion.Lerp(startRotation, targetRotation, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        arm.localRotation = targetRotation;
    }

    private IEnumerator RotateToRotation(Transform arm, Quaternion targetRotation, float duration)
    {
        Quaternion startRotation = arm.localRotation;
        float time = 0f;

        while (time < duration)
        {
            arm.localRotation = Quaternion.Lerp(startRotation, targetRotation, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        arm.localRotation = targetRotation;
    }

    // Chequea si se ganó el jeroglífico
    public void OnWinMethod()
    {
        if (trackerList.All(t => t.HasWon))
        {
            HieroglyphCompletedAction?.Invoke(this);
            StartCoroutine(DisableArms());
            interactorCollider.enabled = false; // Desactiva el collider

            if (subFloor && isTarget)
            {
                hieroglyficManager.CheckToUpdateCounter();
            }
        }
    }

    private IEnumerator DisableArms()
    {
        trail.gameObject.SetActive(false); //Apagamos el rayo
        armHandler.DisableArm(rightArm); //Bajamos la mano y la apagamos al finalizar la corrutina
        yield return new WaitForSeconds(0.5f);
        TurnPuzzleCamera(false);
        BackToGameplay(true); //Volvemos a la camara de gameplay
        HasWon = true;
    }
}
