using System;
using System.Collections;
using UnityEngine;

public class SpherePuzzleManager : MonoBehaviour
{
    public Action<SpherePuzzleManager> OnWinAction;

    [Header("Acciones al ganar")]
    public Animator openDoor;
    [SerializeField] private MeshRenderer sphereRenderer;
    [SerializeField] private WallLaser laser;
    private Material emissiveMat;
    private float distance;
    private bool hasWon;

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
        OnWinAction?.Invoke(this);

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

        if (openDoor != null /*&& laser.isEnabled*/)
        {
            openDoor.SetTrigger("Open");
        }

        yield return new WaitForSeconds(1f);
        EventManager.Instance.Dispatch(GameEventTypes.OnGameplay, this, EventArgs.Empty);
    }
}
