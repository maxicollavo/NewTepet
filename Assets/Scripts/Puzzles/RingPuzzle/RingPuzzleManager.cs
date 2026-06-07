using System;
using System.Collections;
using UnityEngine;

public class RingPuzzleManager : MonoBehaviour
{
    public static RingPuzzleManager Instance;
    public RingPuzzleController controller;

    [SerializeField] private Ring[] rings;

    [HideInInspector] public bool canInteract { get; set; }
    [HideInInspector] public bool isRotating { get; private set; }

    [SerializeField] private float rotationDuration = 2f;
    private int currentSelectedRing = -1;

    [SerializeField] Laser laser;
    private bool hasWon;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (!canInteract || hasWon) return;

        hasWon = laser.hasWon;
        if (hasWon)
        {
            Win();
            return;
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            RotateSelectedRing(45f);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            RotateSelectedRing(-45f);
        }
    }

    public void SelectRing(int index)
    {
        if (currentSelectedRing >= 0)
        {
            rings[currentSelectedRing].isSelected = false;
            rings[currentSelectedRing].DisableOutline();
        }

        currentSelectedRing = index;

        rings[currentSelectedRing].isSelected = true;
        rings[currentSelectedRing].EnableSelectedOutline();
    }

    private void RotateSelectedRing(float amount)
    {
        if (isRotating) return;

        if (currentSelectedRing < 0) return;

        StartCoroutine(
            RotateRing(
                rings[currentSelectedRing].transform.parent,
                amount
            )
        );
    }

    private IEnumerator RotateRing(Transform ringTransform, float amount)
    {
        isRotating = true;
        canInteract = false;

        rings[currentSelectedRing].OnStartRotation();

        DisableAllOutlines();

        Quaternion startRotation = ringTransform.localRotation;

        float targetY =
            ringTransform.localEulerAngles.y + amount;

        Quaternion targetRotation =
            Quaternion.Euler(
                ringTransform.localEulerAngles.x,
                targetY,
                ringTransform.localEulerAngles.z
            );

        float time = 0f;

        while (time < rotationDuration)
        {
            time += Time.deltaTime;

            ringTransform.localRotation = Quaternion.Lerp(
                startRotation,
                targetRotation,
                time / rotationDuration
            );

            yield return null;
        }

        ringTransform.localRotation = targetRotation;

        if (currentSelectedRing >= 0)
        {
            rings[currentSelectedRing].EnableSelectedOutline();
        }

        canInteract = true;
        isRotating = false;
    }

    public void DeselectAll()
    {
        currentSelectedRing = -1;

        foreach (Ring ring in rings)
        {
            ring.DisableOutline();
        }
    }

    private void DisableAllOutlines()
    {
        foreach (Ring ring in rings)
        {
            ring.DisableOutline();
        }
    }

    private void Win()
    {
        StartCoroutine(WinCoroutine());
    }

    private IEnumerator WinCoroutine()
    {
        canInteract = false;
        DeselectAll();
        yield return new WaitForSeconds(1.5f);

        yield return StartCoroutine(controller.ExitPuzzle(false));

        controller.enabled = false;
    }
}
