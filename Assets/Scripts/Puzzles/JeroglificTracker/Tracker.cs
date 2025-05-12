using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tracker : MonoBehaviour
{
    [Header("Start Settings")]
    public bool CanStart;
    public bool firstHieroglyfic;
    [SerializeField] GameObject path;
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
        statueManager.SetNodes += SetNodesMethod;
    }

    //Pos 0 es el primer jeroglifico y pos 1 es el segundo
    private void SetNodesMethod(StatueManager manager, int pos)
    {
        if (firstHieroglyfic)
        {
            if (pos == 1)
            {
                foreach (var node in actionNodes)
                {
                    CanStart = false;
                    node.SetActive(false);
                    path.SetActive(false);
                }
                return;
            }
            else
            {
                foreach (var node in actionNodes)
                {
                    CanStart = true;
                    path.SetActive(true);
                    node.SetActive(true);
                }
            }
        }
        else
        {
            if (pos == 0)
            {
                foreach (var node in actionNodes)
                {
                    Debug.Log("Apaga los nodos");
                    node.SetActive(false);
                    path.SetActive(false);
                    CanStart = false;
                }
                return;
            }
            else
            {
                foreach (var node in actionNodes)
                {
                    Debug.Log("Enciende los nodos");
                    CanStart = true;
                    node.SetActive(true);
                    path.SetActive(true);
                }
            }
        }
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

        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 0.1f);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, nodeLayerMask))
        {
            Debug.Log($"Raycast HIT: '{hit.collider.gameObject.name}' (Layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)}) at distance {hit.distance}");

            GameObject hitObj = hit.collider.gameObject;
            CheckIfValid(hitObj);
        }
        else
        {
            Debug.Log("Raycast MISSED any node.");
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
        DisableNodes();
        manager.OnWinMethod();
    }

    void DisableNodes()
    {
        foreach (var node in actionNodes)
        {
            node.GetComponent<MeshCollider>().enabled = false;
        }
    }

    private void AddNode(GameObject node)
    {
        Debug.Log("Add node");
        currentPath.Add(node);
        var nodeMesh = node.GetComponent<MeshRenderer>();
        currentMeshes.Add(nodeMesh);
        TurnOrRestartNodes(nodeMesh, true);
    }

    private void TurnOrRestartNodes(MeshRenderer mesh, bool state)
    {
        if (state)
        {
            Debug.Log("Enciende mesh");
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