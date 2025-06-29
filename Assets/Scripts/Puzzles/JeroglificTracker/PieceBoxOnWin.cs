using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class PieceBoxOnWin : MonoBehaviour
{
    [Header("References")]
    [SerializeField] TrackerManager manager;
    [SerializeField] CinemachineCamera cinematicCam;
    [SerializeField] BoxCollider interactorColl;
    [SerializeField] BoxCollider pieceColl;
    private CinemachineBrain brain;
    [SerializeField] Animator boxAnim;
    [SerializeField] Transform lookAtTarget;
    Transform originalLookAt;
    [SerializeField] AudioSource WinBoxSound;
    [SerializeField] AudioSource WinSound;
    void Start()
    {
        manager.HieroglyphCompletedAction += Win;

        brain = Camera.main.GetComponent<CinemachineBrain>();
    }

    void Win(TrackerManager manager)
    {
        StartCoroutine(Cinematic());
    }

    private IEnumerator Cinematic()
    {
        manager.trail.gameObject.SetActive(false);           // Apagamos el rayo del jugador
        interactorColl.enabled = false;                      // Desactivamos el collider de interacción

        yield return new WaitForSeconds(0.5f);

        manager.TurnPuzzleCamera(false);                   // Apagamos la cámara de puzzle
        EventManager.Instance.Dispatch(GameEventTypes.OnCinematic, this, EventArgs.Empty); // Ocultamos el cursor o HUD
        cinematicCam.gameObject.SetActive(true);             // Activamos la cámara de cinemática
        AudioManager.Instance.PlaySound("SlideClick");       // Reproducimos sonido de desbloqueo

        yield return new WaitForSeconds(1.3f);

        boxAnim.SetTrigger("Open");                          // Disparamos animación
        //AudioManager.Instance.PlaySound("OpenBox");          // Reproducimos sonido de apertura
        //WinSound.Play();
        WinBoxSound.Play();
        yield return new WaitForSeconds(1f);

        cinematicCam.gameObject.SetActive(false);            // Apagamos la cámara de cinemática
        pieceColl.enabled = true;                            // Activamos el collider de la pieza recompensa

        EventManager.Instance.Dispatch(GameEventTypes.OnGameplay, this, EventArgs.Empty); // Restauramos el HUD/cursor
        manager.BackToGameplay(true);                        // Volvemos a la cámara de gameplay
        manager.HasWon = true;                               // Marcamos que se ganó
    }
}