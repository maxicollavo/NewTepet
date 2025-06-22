using System.Collections;
using TMPro;
using UnityEngine;

public class TutorialTriggers : MonoBehaviour, ITutorial
{
    [SerializeField] Animator door;
    BoxCollider coll;
    [SerializeField] AudioSource door3DSound;
    private void Awake()
    {
        coll = GetComponent<BoxCollider>();
    }

    public void Exit()
    {
        door.SetTrigger("Close");
        //AudioManager.Instance.PlaySound("rocaMoviendose");
        door3DSound.Play();
        coll.enabled = false;
    }
}
