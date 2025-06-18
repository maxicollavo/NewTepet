using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class PieceBoxOnWin : MonoBehaviour
{
    [Header("References")]
    [SerializeField] VertexSignalReceiver receiver;
    [SerializeField] TrackerManager manager;
    [SerializeField] CinemachineCamera cinematicCam;
    [SerializeField] BoxCollider interactorColl;
    [SerializeField] BoxCollider pieceColl;
    private CinemachineBrain brain;
    [SerializeField] Animator boxAnim;
    [SerializeField] Transform lookAtTarget;
    Transform originalLookAt;

    void Start()
    {
        manager.JeroglificAction += Win;

        brain = Camera.main.GetComponent<CinemachineBrain>();
    }

    void Win(TrackerManager manager)
    {
        StartCoroutine(Cinematic());
    }

    private IEnumerator Cinematic()
    {
        manager.trail.gameObject.SetActive(false); //Apagamos el rayo
        receiver.DisableArm(); //Bajamos la mano y la apagamos al finalizar la corrutina
        yield return new WaitForSeconds(0.5f);
        manager.TurnPuzzleCamera(false);
        EventManager.Instance.Dispatch(GameEventTypes.OnCinematic, this, EventArgs.Empty); //Sacamos el cursor
        interactorColl.enabled = false; //Desactivamos el collider del interactor
        cinematicCam.gameObject.SetActive(true); //Encendemos la camara de la cinemática
        AudioManager.Instance.PlaySound("SlideClick"); //Sonido de desbloqueo de caja
        yield return new WaitForSeconds(1.3f);
        //interactorColl.enabled = false;
        boxAnim.SetTrigger("Open"); //Abrimos caja
        AudioManager.Instance.PlaySound("OpenBox"); //Sonido de apertura de caja
        //interactorColl.enabled = false;
        yield return new WaitForSeconds(1f);
        //interactorColl.enabled = false;
        cinematicCam.gameObject.SetActive(false); //Apagamos la camara de cinematica
        pieceColl.enabled = true; //Encendemos el collider de la recompensa
        EventManager.Instance.Dispatch(GameEventTypes.OnGameplay, this, EventArgs.Empty); //Devolvemos el cursor
        manager.BackToGameplay(true); //Volvemos a la camara de gameplay
        manager.HasWon = true;
    }
}