using System.Collections;
using TMPro;
using UnityEngine;

public class TutorialTriggers : MonoBehaviour, ITutorial
{
    [SerializeField] Animator door;
    BoxCollider coll;

    private void Awake()
    {
        coll = GetComponent<BoxCollider>();
    }

    public void Exit()
    {
        door.SetTrigger("Close");
        coll.enabled = false;
    }
}
