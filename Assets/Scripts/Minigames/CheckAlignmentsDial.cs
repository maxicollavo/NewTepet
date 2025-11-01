using System;
using UnityEngine;

public class CheckAlignmentsDial : MonoBehaviour
{
    public RotatableDial[] dials;
    private GameObject[] dialsGos;

    public Action OnDialAligned;

    private void Start()
    {
        dialsGos = new GameObject[dials.Length];

        foreach (var dial in dials)
        {
            dial.OnClickRelease += CheckAlignments;
        }

        for (int i = 0; i < dials.Length; i++)
        {
            if (dials[i].transform.childCount > 0)
            {
                dialsGos[i] = dials[i].transform.GetChild(0).gameObject;
            }
        }
    }
    public RotatableDial[] GetDials() => dials;

    private void CheckAlignments()
    {
        Debug.Log("Se suelta el mouse");
        float[] angles = new float[dials.Length];
        for (int i = 0; i < dials.Length; i++)
        {
            angles[i] = dials[i].transform.eulerAngles.z;
        }

        float tolerance = 5f;
        bool aligned = AreAnglesAligned(angles, tolerance);

        if (aligned)
        {
            DialsAligned();
        }
    }

    private bool AreAnglesAligned(float[] angles, float tolerance)
    {
        float reference = angles[0];

        for (int i = 1; i < angles.Length; i++)
        {
            float diff = Mathf.Abs(Mathf.DeltaAngle(reference, angles[i]));
            if (diff > tolerance)
                return false;
        }

        return true;
    }

    private void DialsAligned()
    {
        OnDialAligned?.Invoke();
    }
}
