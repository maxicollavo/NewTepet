using System;
using System.Collections;
using UnityEngine;

public class ColumnsOnWin : MonoBehaviour
{
    [Header("If there are 2 columns")]
    [SerializeField] ColumnInteractManager manager;
    [SerializeField] EnterColumnPuzzle enterPuzzle;
    [SerializeField] Animator boxAnim;
    [SerializeField] ParticleSystem ColumnsWin;

    [Header("If there are 4 columns")]
    public bool areFourColumns;
    [SerializeField] PickToInventory spherePick;

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
        ColumnsWin.Play();
        yield return new WaitForSeconds(1f);
        enterPuzzle.EnterPuzzle(false);
        yield return new WaitForSeconds(0.1f);
        enterPuzzle.canInteract = false;

        if (areFourColumns)
        {
            spherePick.canBePicked = true;
        }
    }
}
