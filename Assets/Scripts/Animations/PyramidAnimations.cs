using System.Collections;
using UnityEngine;

public class PyramidAnimations : MonoBehaviour
{
    [Header("State")]
    public bool isFloating = true;

    [Header("Idle Floating Settings")]
    [Tooltip("Speed of the up and down cycle")]
    public float frequency = 3f;

    [Tooltip("Maximum vertical distance in meters")]
    public float amplitude = 0.04f;

    [Tooltip("How fast it returns to its resting position when stopped")]
    public float transitionSmoothing = 5f;

    [Header("Optional Rotation")]
    public float rotationSpeed = 50f;

    [Header("Interaction Animation")]
    [Tooltip("Local direction and distance the pyramid moves when interacting (Z is forward)")]
    public Vector3 interactDirection = new Vector3(0f, 0f, 0.25f);

    [Tooltip("Total duration of the forward-and-back interaction motion")]
    public float interactDuration = 0.5f;

    private Vector3 initialLocalPosition;
    private Quaternion initialLocalRotation;
    private float floatingFactor = 1f;

    // Additive offset controlled by the interaction coroutine
    private Vector3 interactOffset = Vector3.zero;
    private Coroutine interactCoroutine;
    void Start()
    {
        initialLocalPosition = transform.localPosition;
        initialLocalRotation = transform.localRotation;
    }

    void Update()
    {
        // 1. Smooth transition between floating (1) and resting (0)
        float targetFactor = isFloating ? 1f : 0f;
        floatingFactor = Mathf.MoveTowards(floatingFactor, targetFactor, Time.deltaTime * transitionSmoothing);

        // 2. Calculate vertical floating offset
        float yFloatOffset = Mathf.Sin(Time.time * frequency) * amplitude * floatingFactor;
        Vector3 floatOffset = new Vector3(0f, yFloatOffset, 0f);

        // 3. Combine base position + current floating height + interaction forward thrust
        transform.localPosition = initialLocalPosition + floatOffset + interactOffset;

        // 4. Rotation
        if (floatingFactor > 0.01f)
        {
            transform.Rotate(Vector3.up, rotationSpeed * floatingFactor * Time.deltaTime, Space.Self);
        }
        else
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, initialLocalRotation, Time.deltaTime * transitionSmoothing);
        }
    }

    // Call this method when the player interacts with an object
    public void PlayInteractAnimation()
    {
        if (!gameObject.activeInHierarchy) return;

        if (interactCoroutine != null)
        {
            StopCoroutine(interactCoroutine);
        }
        interactCoroutine = StartCoroutine(InteractRoutine());
    }

    private IEnumerator InteractRoutine()
    {
        float elapsed = 0f;

        while (elapsed < interactDuration)
        {
            elapsed += Time.deltaTime;
            float normalizedTime = Mathf.Clamp01(elapsed / interactDuration);

            // Mathf.Sin(normalizedTime * Mathf.PI) creates a smooth curve: 0 -> 1 -> 0
            float curve = Mathf.Sin(normalizedTime * Mathf.PI);
            interactOffset = interactDirection * curve;

            yield return null;
        }

        interactOffset = Vector3.zero;
        interactCoroutine = null;
    }

    public void StartFloating() => isFloating = true;
    public void StopFloating() => isFloating = false;
}