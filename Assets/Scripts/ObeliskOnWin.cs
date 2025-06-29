using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObeliskOnWin : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] ColumnsOnWin Columns;
    [SerializeField] SpherePuzzleManager Sphere;
    [SerializeField] LevelThreeHieroglyficsOnWin Owl;
    [SerializeField] RiverPuzzleManager Board;
    [SerializeField] Transform obeliskTransform;
    [SerializeField] CameraShake shake;
    [SerializeField] AudioSource ObeliskMoveSound;


    private int counter;
    private bool hasWon;

    private Vector3 initialPosition;
    private float targetDepth = -12f;
    private float moveDuration = 5f;

    private void Awake()
    {
        Columns.ColumnsOnWinAction += UpdateObelisk;
        Sphere.SphereCompletedAction += UpdateObelisk;
        Owl.OnWinAction += UpdateObelisk;
        Board.OnWin += UpdateObelisk;

        initialPosition = obeliskTransform.position;
    }

    private void UpdateObelisk()
    {
        if (hasWon || counter > 3) return;

        float stepSize = targetDepth / 4f;
        float targetY = obeliskTransform.position.y + stepSize;

        Vector3 targetPosition = new Vector3(
            obeliskTransform.position.x,
            targetY,
            obeliskTransform.position.z
        );

        StartCoroutine(MoveObelisk(obeliskTransform.position, targetPosition));

        counter++;

        if (counter >= 4)
        {
            hasWon = true;
        }
    }

    private IEnumerator MoveObelisk(Vector3 start, Vector3 end)
    {
        shake.TriggerShake(moveDuration, 0.1f);
        ObeliskMoveSound.Play();
        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);
            obeliskTransform.position = Vector3.Lerp(start, end, t);
            yield return null;
        }

        obeliskTransform.position = end;
    }

}