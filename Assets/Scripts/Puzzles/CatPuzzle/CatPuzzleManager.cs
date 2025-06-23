using UnityEngine;

public class CatPuzzleManager : MonoBehaviour
{
    [SerializeField] StatueInteractor interactor;
    [SerializeField] GameObject lights;
    [SerializeField] GameObject path;
    [SerializeField] GameObject[] colliders;

    private bool state;

    private void Awake()
    {
        interactor.InteractorAction += OnInteractMethod;
    }

    private void OnInteractMethod(StatueInteractor interactor)
    {
        state = !state;
        lights.SetActive(state);
        path.SetActive(state);
        foreach (var collider in colliders)
            collider.SetActive(state);
    }
}
