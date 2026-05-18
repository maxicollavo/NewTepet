using UnityEngine;

public class RingPuzzleManager : MonoBehaviour
{
    public static RingPuzzleManager Instance;

    [HideInInspector] public bool canInteract { get; set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
