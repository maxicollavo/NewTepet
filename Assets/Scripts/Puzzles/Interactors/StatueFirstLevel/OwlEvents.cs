using UnityEngine;

public class OwlEvents : MonoBehaviour
{
    [SerializeField] StatueManager manager;

    public void AnimCallback()
    {
        manager.SetCollider();
    }
}
