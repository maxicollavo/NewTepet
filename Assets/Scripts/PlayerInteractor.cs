using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out ITutorial tutorial))
        {
            tutorial.Exit();
        }

        if (other.TryGetComponent(out IEnter enterLevel))
        {
            enterLevel.Enter();
        }
    }
}