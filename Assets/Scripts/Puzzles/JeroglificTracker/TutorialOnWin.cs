using System.Collections;
using UnityEngine;

public class TutorialOnWin : MonoBehaviour
{
    [SerializeField] TrackerManager manager;

    [SerializeField] Animator doorAnim;
    [SerializeField] AudioSource door3DSound;
   

    void Start()
    {
        manager.HieroglyphCompletedAction += Win;
    }

    void Win(TrackerManager manager)
    {
        StartCoroutine(OpenDoorCoroutine());
    }

    public IEnumerator OpenDoorCoroutine()
    {
        yield return new WaitForSeconds(1.7f);
        doorAnim.SetTrigger("Open");
        yield return new WaitForSeconds(0.025f);
        door3DSound.Play();
    }
}
