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
        yield return new WaitForSeconds(1.2f);
        doorAnim.SetTrigger("Open");
        //AudioManager.Instance.PlaySound("rocaMoviendose");
        door3DSound.Play();
    }
}
