using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereLaserInteractor : MonoBehaviour, ILaserInteractor
{
    private RotateSphere rotateSphere;

    private BoxCollider coll;

    private void Awake()
    {
        rotateSphere = transform.parent.GetComponent<RotateSphere>();
    }

    private void Start()
    {
        rotateSphere.SphereOnWinAction += EnableGameObject;
    }

    private void EnableGameObject(RotateSphere sphere)
    {
        coll.enabled = true;
    }

    public void Interact()
    {
        //Apertura de puerta, sonidos, cinematica, etc
        Debug.Log("Laser interactua");
    }
}
