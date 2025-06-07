using System;
using System.Collections;
using UnityEngine;

public class ColumnsOnWin : MonoBehaviour
{
    [SerializeField] ColumnInteractManager manager;
    [SerializeField] EnterColumnPuzzle enterPuzzle;
    [SerializeField] Animator boxAnim;

    private void Awake()
    {
        manager.OnWinAction += OnWinMethod;
    }

    private void OnWinMethod(ColumnInteractManager manager)
    {
        StartCoroutine(OnWinCoroutine());   
    }

    private IEnumerator OnWinCoroutine()
    {
        boxAnim.SetTrigger("Open");
        //Sonido de apertura de caja
        yield return new WaitForSeconds(1f);
        enterPuzzle.EnterPuzzle(false);
        yield return new WaitForSeconds(0.1f);
        enterPuzzle.canInteract = false;
    }
}
