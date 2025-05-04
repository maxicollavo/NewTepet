using UnityEngine;
using System.Collections;

public class DisolveController : MonoBehaviour
{/*
    [SerializeField] float dissolverRate = 0.0125f;
    [SerializeField] float refreshRate = 0.025f;
    [SerializeField] Renderer MeshRender;
    [SerializeField] Material[] Materials;
    private bool isDissolving = false;
    private bool isRestoring = false;

    void Start()
    {
        if (MeshRender != null)
            Materials = MeshRender.materials;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && !isDissolving && !isRestoring)
        {
            StartCoroutine(DissolveCo());
        }
        else if (Input.GetKeyDown(KeyCode.K) && !isDissolving && !isRestoring)
        {
            StartCoroutine(RestoreCo());
        }
    }

    public IEnumerator DissolveCo()
    {
        Debug.Log("entro a dissolve");
        if (Materials.Length > 0)
        {
            isDissolving = true;
            float counter = 0;
            while (counter < 1)
            {
                counter += dissolverRate;
                for (int i = 0; i < Materials.Length; i++)
                {
                    Materials[i].SetFloat("_DissolverAmount", counter);
                }
                yield return new WaitForSeconds(refreshRate);
            }
            isDissolving = false;
        }
    }

    public IEnumerator RestoreCo()
    {
        Debug.Log("entro a Restore");
        if (Materials.Length > 0)
        {
            isRestoring = true;
            float counter = 1;
            while (counter > 0)
            {
                counter -= dissolverRate;
                for (int i = 0  ; i < Materials.Length; i++)
                {
                    Materials[i].SetFloat("_DissolverAmount", counter);
                }
                yield return new WaitForSeconds(refreshRate);
            }
            isRestoring = false;
        }
    }

    public void SetDissolveAmount(float amount)
    {
        for (int i = 0; i < Materials.Length; i++)
        {
            Materials[i].SetFloat("_DissolverAmount", amount);
        }
    }*/

    [Header("Dissolve Settings")]
    [SerializeField] float dissolverRate = 0.0125f;
    [SerializeField] float refreshRate = 0.025f;

    [SerializeField] Renderer MeshRender;

    void Start()
    {
        if (MeshRender == null)
            MeshRender = GetComponent<Renderer>();
    }

    public IEnumerator DissolveAndRestore(DisolveController other)
    {
        float counter = 0f;

        Material[] myMats = MeshRender.materials;
        Material[] otherMats = other.MeshRender.materials;

        while (counter <= 1f)
        {
            foreach (var mat in myMats)
                mat.SetFloat("_DissolverAmount", counter);

            foreach (var mat in otherMats)
                mat.SetFloat("_DissolverAmount", 1f - counter);

            counter += dissolverRate;
            yield return new WaitForSeconds(refreshRate);
        }
    }

    public void SetDissolveAmount(float amount)
    {
        if (MeshRender == null) return;

        foreach (var mat in MeshRender.materials)
        {
            mat.SetFloat("_DissolverAmount", amount);
        }
    }
}

