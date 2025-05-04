using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AnimacionesPyramid : MonoBehaviour
{
    [SerializeField] ParticleSystem chispas;
    [SerializeField] GameObject top;

    [SerializeField] Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        anim.SetTrigger("Start");
    }
    void Close()
    {
        Instantiate(chispas, top.transform);
    }

    public void RestartAnim()
    {
        Debug.Log("Reinicia anim piramide");
        //anim.Play("PyramidStartLevitation", -1, 0f);
    }
}
