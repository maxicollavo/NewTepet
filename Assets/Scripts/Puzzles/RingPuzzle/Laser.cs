using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [Header("Settings")]
    public LayerMask mirrorMask;
    public LayerMask winnerMask;
    public LineRenderer line;
    public float maxDistance = 128;
    public int maxBounces = 128;

    [SerializeField] private float surfaceOffset = 0.01f;

    [Header("State")]
    readonly List<Vector3> _linePoints = new();
    public bool hasWon { get; private set; }
    private int mirrorCounter;

    private void Update()
    {
        Physics.SyncTransforms();

        mirrorCounter = 0;

        _linePoints.Clear();
        _linePoints.Add(transform.position);

        ShootLaser(transform.position, transform.forward, maxBounces);

        line.positionCount = _linePoints.Count;
        line.SetPositions(_linePoints.ToArray());
    }

    void ShootLaser(Vector3 pos, Vector3 dir, int bounceLimit)
    {
        if (bounceLimit <= 0)
            return;

        dir = dir.normalized;

        int hitMask = mirrorMask.value | winnerMask.value;

        if (!Physics.Raycast(pos, dir, out RaycastHit hit, maxDistance, hitMask))
        {
            _linePoints.Add(pos + dir * maxDistance);
            return;
        }

        _linePoints.Add(hit.point);

        GameObject target = hit.collider.gameObject;

        if (IsObjectMirror(target))
        {
            mirrorCounter++;

            Vector3 reflectedDir = Vector3.Reflect(dir, hit.normal).normalized;
            Vector3 newStartPos = hit.point + reflectedDir * surfaceOffset;

            ShootLaser(newStartPos, reflectedDir, bounceLimit - 1);

            return;
        }

        if (IsObjectTargetWinner(target))
        {
            if (mirrorCounter == 3 && !hasWon)
            {
                hasWon = true;
            }

            return;
        }
    }

    bool IsObjectMirror(GameObject target)
    {
        return (mirrorMask.value & (1 << target.layer)) != 0;
    }

    bool IsObjectTargetWinner(GameObject target)
    {
        return (winnerMask.value & (1 << target.layer)) != 0;
    }
}