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

    [Header("State")]
    readonly List<Vector3> _linePoints = new();
    private bool hasWon;

    private void Update()
    {
        if (hasWon)
            return;

        _linePoints.Clear();
        _linePoints.Add(transform.position);
        ShootLaser(transform.position, transform.forward, maxBounces);
        line.positionCount = _linePoints.Count;
        line.SetPositions(_linePoints.ToArray());
    }

    void ShootLaser(Vector3 pos, Vector3 dir, int bounceLimit)
    {
        if (bounceLimit == 0)
            return;

        bounceLimit--;
        dir = dir.normalized;

        if (!Physics.Raycast(pos, dir, out RaycastHit hit, maxDistance))
        {
            _linePoints.Add(pos + dir * maxDistance);
            return;
        }

        _linePoints.Add(hit.point);
        var target = hit.collider.gameObject;

        if (IsObjectMirror(target))
        {
            var reflectedDir = Vector3.Reflect(dir, hit.normal);
            ShootLaser(hit.point, reflectedDir, bounceLimit);
        }

        if (IsObjectTargetWinner(target))
        {
            hasWon = true;
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