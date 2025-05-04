using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tracker : MonoBehaviour
{
    [Header("Start Settings")]
    public bool CanStart;
    [SerializeField] StatueManager statueManager;

    [Header("Nodes")]
    [SerializeField] List<GameObject> actionNodes; //Referencia a su action
    List<GameObject> validNodes; //Los nodos que tengo que tocar
    private List<GameObject> currentPath = new List<GameObject>(); //Los nodos que toco
    private List<MeshRenderer> currentMeshes = new List<MeshRenderer>(); //Todos los mesh renderer que toco
    [SerializeField] private LayerMask nodeLayerMask;

    [Header("Particle")]
    ParticleSystem particle;

    [Header("Tracker")]
    public bool isTracking { get; private set; }
    [SerializeField] GameObject tracker;

    [Header("Settings")]
    public TrackerManager manager;
    private Camera playerCam;
    private bool previousState = false;

    [Header("On Win")]
    public bool HasWon;


    private void Start()
    {
        if (validNodes == null || validNodes.Count == 0)
        {
            validNodes = new List<GameObject>(actionNodes);
        }

        playerCam = Camera.main;

        particle = tracker.GetComponent<ParticleSystem>();

        manager.trackerList.Add(this);

        if (statueManager == null) return;
        statueManager.StatueManagerAction += OnStatueFinish;
    }

    private void OnStatueFinish(StatueManager manager)
    {
        CanStart = true;
    }

    void Update()
    {
        if (HasWon || !CanStart) return;

        bool currentState = manager.OnPuzzle;

        if (currentState != previousState)
        {
            tracker.SetActive(currentState);
            previousState = currentState;
        }

        if (!currentState) return;

        if (Input.GetMouseButtonDown(0))
        {
            currentPath.Clear();
            isTracking = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isTracking = false;
            RestartTracking();
        }

        if (isTracking)
        {
            TrackMouse();
        }

        ParticleTracking(isTracking);
    }

    private void TrackMouse()
    {
        Ray ray = playerCam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, nodeLayerMask))
        {
            GameObject hitObj = hit.collider.gameObject;

            CheckIfValid(hitObj);
        }
        else
        {
            RestartTracking();
        }
    }

    private void RestartTracking()
    {
        isTracking = false;
        currentPath.Clear();
        TurnOrRestartNodes(null, false);
    }

    private void CheckIfValid(GameObject node)
    {
        if (validNodes.Contains(node) && !currentPath.Contains(node))
        {
            AddNode(node);
            CheckWin();
        }
    }

    private void CheckWin()
    {
        if (currentPath.Count == validNodes.Count)
        {
            isTracking = false;
            Win();
        }
    }

    private void Win()
    {
        HasWon = true;
        manager.OnWinMethod();
    }

    private void AddNode(GameObject node)
    {
        currentPath.Add(node);
        var nodeMesh = node.GetComponent<MeshRenderer>();
        currentMeshes.Add(nodeMesh);
        TurnOrRestartNodes(nodeMesh, true);
    }

    private void TurnOrRestartNodes(MeshRenderer mesh, bool state)
    {
        if (state)
        {
            mesh.enabled = true;
        }
        else
        {
            foreach (var item in currentMeshes)
            {
                item.enabled = false;
            }
        }
    }

    private void ParticleTracking(bool isTracking)
    {
        if (isTracking)
        {
            if (!particle.isPlaying)
            {
                particle.Play();
            }
        }
        else
        {
            if (particle.isPlaying)
            {
                particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            }
        }
    }
}