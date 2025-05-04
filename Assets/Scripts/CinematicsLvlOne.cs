using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;

public class CinematicsLvlOne : MonoBehaviour
{
    [Header("Action Receiving")]
    [SerializeField] BoardPuzzleManager boardManager;

    [Header("References")]
    [SerializeField] PlayableDirector boardDirector;
    private CinemachineBrain brain;

    private bool waitingForBlendEndBoard;

    private void Start()
    {
        brain = Camera.main.GetComponent<CinemachineBrain>();

        boardManager.OnWin += OnBoardWin;
    }

    private void Update()
    {
        if (!waitingForBlendEndBoard) return;

        if (!brain.IsBlending)
        {
            boardDirector.Play();
            waitingForBlendEndBoard = false;
        }
    }

    public void OnBoardWin(BoardPuzzleManager manager)
    {
        waitingForBlendEndBoard = true;
    }
}