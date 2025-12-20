using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandsManager : MonoBehaviour
{
    public static HandsManager Instance;

    [Header("Arms")]
    [SerializeField] private Transform leftArm;
    [SerializeField] private Transform rightArm;

    [Header("Timing")]
    [SerializeField] private float timer = 0.5f;
    public float PoseDuration => timer;

    private readonly Dictionary<Transform, Quaternion> originalRotations = new();
    private readonly Dictionary<Transform, Coroutine> running = new();

    private void Awake()
    {
        Instance = this;

        // Guardar originales lo antes posible (como tu TrackerManager.Start)
        CacheOriginal(leftArm);
        CacheOriginal(rightArm);
    }

    private void CacheOriginal(Transform arm)
    {
        if (arm == null) return;
        if (!originalRotations.ContainsKey(arm))
            originalRotations.Add(arm, arm.localRotation);
    }

    // Si por algún motivo querés "redefinir" la pose original (ej. luego de una animación)
    public void ResetOriginal(ArmTarget target)
    {
        foreach (var arm in GetArms(target))
        {
            if (arm == null) continue;
            originalRotations[arm] = arm.localRotation;
        }
    }

    // =========================
    // API PÚBLICA
    // =========================

    public void SetPose(HandPose pose, ArmTarget target = ArmTarget.Both)
    {
        switch (pose)
        {
            case HandPose.Hieroglyfic:
                RotateToX(target, 90f, timer);
                break;

            case HandPose.Puzzle:
                RotateToX(target, 90f, timer);
                break;

            case HandPose.Gameplay:
                Restore(target, timer);
                break;
        }
    }

    public void RotateToX(ArmTarget target, float targetX, float duration)
    {
        foreach (var arm in GetArms(target))
            StartArmRoutine(arm, CoRotateArmToX(arm, targetX, duration));
    }

    public void Restore(ArmTarget target, float duration)
    {
        foreach (var arm in GetArms(target))
        {
            if (arm == null) continue;

            // Aseguramos que existe original sí o sí
            CacheOriginal(arm);

            Quaternion original = originalRotations[arm];
            StartArmRoutine(arm, CoRotateToRotation(arm, original, duration));
        }
    }

    // =========================
    // Internals
    // =========================

    private IEnumerable<Transform> GetArms(ArmTarget target)
    {
        switch (target)
        {
            case ArmTarget.Left:
                return new[] { leftArm };

            case ArmTarget.Right:
                return new[] { rightArm };

            default:
                return new[] { leftArm, rightArm };
        }
    }

    private void StartArmRoutine(Transform arm, IEnumerator routine)
    {
        if (arm == null) return;

        if (running.TryGetValue(arm, out var c) && c != null)
            StopCoroutine(c);

        running[arm] = StartCoroutine(routine);
    }

    private IEnumerator CoRotateArmToX(Transform arm, float targetX, float duration)
    {
        if (arm == null) yield break;

        Quaternion startRotation = arm.localRotation;
        Vector3 currentEuler = arm.localEulerAngles;
        Quaternion targetRotation = Quaternion.Euler(targetX, currentEuler.y, currentEuler.z);

        float time = 0f;
        while (time < duration)
        {
            arm.localRotation = Quaternion.Lerp(startRotation, targetRotation, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        arm.localRotation = targetRotation;
    }

    private IEnumerator CoRotateToRotation(Transform arm, Quaternion targetRotation, float duration)
    {
        if (arm == null) yield break;

        Quaternion startRotation = arm.localRotation;

        float time = 0f;
        while (time < duration)
        {
            arm.localRotation = Quaternion.Lerp(startRotation, targetRotation, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        arm.localRotation = targetRotation;
    }
}

public enum HandPose
{
    None,
    Hieroglyfic,
    Puzzle,
    Gameplay
}

public enum ArmTarget
{
    Left,
    Right,
    Both
}
