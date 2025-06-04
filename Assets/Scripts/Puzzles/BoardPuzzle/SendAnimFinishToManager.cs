using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendAnimFinishToManager : MonoBehaviour
{
    [SerializeField] BoardPuzzleManager manager;

    public void SendAnimFinish()
    {
        manager.SetPieceOnBoard();
    }
}