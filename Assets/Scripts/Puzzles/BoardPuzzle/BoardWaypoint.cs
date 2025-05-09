using System.Collections.Generic;
using UnityEngine;

public class BoardWaypoint : MonoBehaviour
{
    public Dictionary<Vector2, BoardWaypoint> neighbors = new Dictionary<Vector2, BoardWaypoint>();

    public BoardWaypoint waypointUp;
    public BoardWaypoint waypointDown;
    public BoardWaypoint waypointLeft;
    public BoardWaypoint waypointRight;

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