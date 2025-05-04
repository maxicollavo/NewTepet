using UnityEngine;

public class TutorialOnWin : MonoBehaviour
{
    [SerializeField] TrackerManager manager;

    [SerializeField] Animator doorAnim;

    void Start()
    {
        manager.JeroglificAction += Win;
    }

    void Win(TrackerManager manager)
    {
        doorAnim.SetTrigger("Open");
    }
}
