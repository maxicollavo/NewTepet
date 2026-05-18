using System;
using System.Collections;
using UnityEngine;

public class RingPuzzleManager : MonoBehaviour
{
    public static RingPuzzleManager Instance;

    [SerializeField] private Ring[] rings;

    [HideInInspector] public bool canInteract { get; set; }
    [HideInInspector] public bool isRotating { get; private set; }

    private float currentYRotation;
    [SerializeField] private float rotationDuration = 2f;
    private int currentSelectedRing = -1;

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
        if (!canInteract) return;

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
        currentSelectedRing = index;
    }

    private void RotateSelectedRing(float amount)
    {
        if (isRotating) return;

        if (currentSelectedRing < 0) return;

        currentYRotation += amount;

        StartCoroutine(
            RotateRing(
                rings[currentSelectedRing].transform.parent
            )
        );
    }

    private IEnumerator RotateRing(Transform ringTransform)
    {
        isRotating = true;

        Quaternion startRotation = ringTransform.localRotation;

        Quaternion targetRotation =
            Quaternion.Euler(
                ringTransform.localEulerAngles.x,
                currentYRotation,
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
}
