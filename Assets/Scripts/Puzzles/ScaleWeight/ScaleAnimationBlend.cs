using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleAnimationBlend : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private float maxWeightDifference = 50f;
    [SerializeField] private float smoothSpeed = 2f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (WeightManager.Instance == null) return;

        float left = WeightManager.Instance.leftWeight;
        float right = WeightManager.Instance.rightWeight;
        float difference = Mathf.Abs(left - right);

        float normalized = Mathf.Clamp01(difference / maxWeightDifference);
        normalized = Mathf.Pow(normalized, 0.5f);

        float targetBlend = 0.5f;

        if (left > right)
            targetBlend = Mathf.Lerp(0.5f, 0f, normalized);
        else if (right > left)
            targetBlend = Mathf.Lerp(0.5f, 1f, normalized);

        float currentBlend = animator.GetFloat("Blend");
        float newBlend = Mathf.Lerp(currentBlend, targetBlend, Time.deltaTime * smoothSpeed);
        animator.SetFloat("Blend", newBlend);
    }

}
