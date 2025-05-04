using Unity.VisualScripting;
using UnityEngine;

public class PieceBoxOnWin : MonoBehaviour
{
    [SerializeField] TrackerManager manager;

    [SerializeField] Animator boxAnim;

    void Start()
    {
        manager.JeroglificAction += Win;
    }

    void Win(TrackerManager manager)
    {
        boxAnim.SetTrigger("Open");
    }
}