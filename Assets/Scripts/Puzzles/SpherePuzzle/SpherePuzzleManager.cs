using System;
using System.Collections;
using UnityEngine;

public class SpherePuzzleManager : MonoBehaviour
{
    [Header("General References")]
    public Action<SpherePuzzleManager> OnWinAction;
    private bool hasWon;
    public Animator mimicAnim;
    [SerializeField] private MeshRenderer sphereRenderer;
    private Material emissiveMat;

    [Header("If is on level one")]
    public Animator openDoor;
    public AudioSource WinSound;

    [Header("If is on level three")]
    public bool isOnLevelThree;
    [SerializeField] BoxCollider owlColl;
    [SerializeField] GameObject owlLights;
    public Action SphereCompletedAction;

    private void Start()
    {
        foreach (var mat in sphereRenderer.sharedMaterials)
        {
            if (mat.name.Contains("Emissive"))
            {
                emissiveMat = mat;
                break;
            }
        }
    }

    public void OnWin()
    {
        if (hasWon) return;
        hasWon = true;
        Debug.Log("¡Puzzle resuelto!");
        emissiveMat = sphereRenderer.material;
        StartCoroutine(WinSequence());
    }

    private IEnumerator WinSequence()
    {
        int blinkCount = 3;

        for (int i = 0; i < blinkCount; i++)
        {
            emissiveMat.SetColor("_EmissionColor", Color.yellow);
            yield return new WaitForSeconds(0.3f);

            emissiveMat.SetColor("_EmissionColor", Color.black);
            yield return new WaitForSeconds(0.3f);
        }

        emissiveMat.SetColor("_EmissionColor", Color.yellow);
        yield return new WaitForSeconds(0.5f);
        if (isOnLevelThree)
        {
            SphereCompletedAction?.Invoke();
            owlColl.enabled = true;
            owlLights.SetActive(true);
        }
        else
        {
            openDoor.SetTrigger("Open");
            EventManager.Instance.Dispatch(GameEventTypes.OnDoorOpen, this, EventArgs.Empty);
            WinSound.Play();
        }

        yield return new WaitForSeconds(1f);
        mimicAnim.SetBool("CanStart", false);
        EventManager.Instance.Dispatch(GameEventTypes.OnGameplay, this, EventArgs.Empty);
    }
}
