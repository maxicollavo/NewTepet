using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverWaypoint : MonoBehaviour
{
    public Dictionary<Vector2, RiverWaypoint> neighbors = new Dictionary<Vector2, RiverWaypoint>();

    public RiverWaypoint waypointUp;
    public RiverWaypoint waypointDown;
    public RiverWaypoint waypointLeft;
    public RiverWaypoint waypointRight;

    public bool IsUsing;

    public bool IsTarget { get; private set; }

    void Awake()
    {
        if (waypointUp != null) neighbors[Vector2.up] = waypointUp;
        if (waypointDown != null) neighbors[Vector2.down] = waypointDown;
        if (waypointLeft != null) neighbors[Vector2.left] = waypointLeft;
        if (waypointRight != null) neighbors[Vector2.right] = waypointRight;
    }
}
