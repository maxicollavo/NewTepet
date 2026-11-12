using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateElevator : MonoBehaviour, Interactor
{
    Outline outline;
    [SerializeField] List<GameObject> boundaries;
    [SerializeField] Animator activateElevator;

    private void Awake()
    {
        outline = GetComponent<Outline>();
    }

    private void Start()
    {
        outline.enabled = false;
    }

    public void Aiming()
    {
        EnableOutline();
    }

    public void DisableOutline()
    {
        outline.enabled = false;
    }
    void EnableOutline()
    {
        outline.enabled = true;
    }

    private IEnumerator BoundariesOff()
    {
        yield return new WaitForSeconds(5f);
        foreach (GameObject go in boundaries)
        {
            go.SetActive(false);
        }
    }

    public void Interact()
    {
        Debug.Log("Toco");
        foreach (GameObject go in boundaries)
        {
            go.SetActive(true);
        }

        activateElevator.SetTrigger("Activate");
    }
}
