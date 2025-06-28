using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MimicSphereActions : MonoBehaviour
{
    [SerializeField] RotateSphere rotateSphere;
    [SerializeField] GameObject mimicSphere;
    MeshRenderer mimicSphereRend;
    Material mimicSphereFillMat;
    Animator mimicSphereAnim;

    WaitForSeconds wfs = new WaitForSeconds(1f);

    private void OnEnable()
    {
        mimicSphereRend = mimicSphere.GetComponent<MeshRenderer>();
        mimicSphereAnim = GetComponent<Animator>();
        mimicSphereFillMat = mimicSphereRend.materials[1];
    }

    public void PlayRotationSound()
    {
        //AudioManager.Instance.PlaySound("EsferaRotando");
    }

    public void RestartSphere()
    {
        StartCoroutine(RestartSphereCoroutine());
    }

    public IEnumerator RestartSphereCoroutine()
    {
        mimicSphereAnim.SetBool("CanStart", false);
        mimicSphereFillMat.SetFloat("_FillAmount", 0f);
        yield return wfs;
        mimicSphereFillMat.SetFloat("_FillAmount", 2f);
        yield return wfs;
        mimicSphereFillMat.SetFloat("_FillAmount", 0f);
        yield return wfs;
        mimicSphereFillMat.SetFloat("_FillAmount", 2f);
        yield return wfs;
        mimicSphereFillMat.SetFloat("_FillAmount", 0f);
        yield return wfs;
        mimicSphereFillMat.SetFloat("_FillAmount", 2f);
        yield return wfs;
        if (rotateSphere.hasWon)
            mimicSphereAnim.SetBool("CanStart", false);
        else
            mimicSphereAnim.SetBool("CanStart", true);
    }
}
