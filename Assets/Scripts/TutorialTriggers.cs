using System.Collections;
using TMPro;
using UnityEngine;

public class TutorialTriggers : MonoBehaviour, ITutorial
{
    [SerializeField] Animator door;
    BoxCollider coll;
    [SerializeField] AudioSource door3DSound;

    [SerializeField] ScenesManager sceneManager;
    public bool goToMenu;

    private void Awake()
    {
        coll = GetComponent<BoxCollider>();
    }

    public void Exit()
    {
        if (goToMenu)
        {
            sceneManager.GoToMenu();
            return;
        }    

        door.SetTrigger("Close");
        //AudioManager.Instance.PlaySound("rocaMoviendose");
        door3DSound.Play();
        coll.enabled = false;
    }
}
