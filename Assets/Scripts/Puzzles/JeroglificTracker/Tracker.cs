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
    [SerializeField] List<Node> actionNodes; //Referencia a su action
    List<Node> validNodes; //Los nodos que tengo que tocar
    private List<Node> currentPath = new List<Node>(); //Los nodos que toco
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
    private bool isOnPause;

    [Header("On Win")]
    public bool HasWon;

    private void OnEnable()
    {
        NewEventManager.OnPaused += PauseTriggered;
    }

    private void OnDisable()
    {
        NewEventManager.OnPaused -= PauseTriggered;
    }

    private void PauseTriggered(bool state)
    {
        isOnPause = state;
    }

    private void Start()
    {
        if (validNodes == null || validNodes.Count == 0)
        {
            validNodes = new List<Node>(actionNodes);
        }

        playerCam = Camera.main;

        particle = tracker.GetComponent<ParticleSystem>();

        //Añadimos el o los trackers del jeroglifico a la lista de su propio manager
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
                CanStart = false;
                path.SetActive(false);
                foreach (var node in actionNodes)
                {
                    node.gameObject.SetActive(false);
                }
                return;
            }
            else
            {
                CanStart = true;
                path.SetActive(true);
                foreach (var node in actionNodes)
                {
                    node.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            if (pos == 0)
            {
                CanStart = false;
                path.SetActive(false);
                foreach (var node in actionNodes)
                {
                    node.gameObject.SetActive(false);
                }
                return;
            }
            else
            {
                CanStart = true;
                path.SetActive(true);
                foreach (var node in actionNodes)
                {
                    node.gameObject.SetActive(true);
                }
            }
        }
    }
    void Update()
    {
        if (HasWon || !CanStart || !manager.OnPuzzle || isOnPause) return;

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
            Node hitObj = hit.collider.GetComponent<Node>();
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
        TurnOrRestartNodes(null, false);
        currentPath.Clear();
        currentMeshes.Clear(); // Limpiamos la lista de meshes también
    }

    private void CheckIfValid(Node node)
    {
        if (node == null || !validNodes.Contains(node) || currentPath.Contains(node))
            return;

        if (currentPath.Count == 0)
        {
            // Si es el primer nodo, debe ser el que tenga isFirst == true
            if (!node.isFirst)
            {
                // Reiniciamos el tracking si no empieza por el primer nodo correcto
                RestartTracking();
                return;
            }
        }

        AddNode(node);
        CheckWin();
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
        HasWon = true; //Para que no vuelva a entrar al update
        DisableNodes(); //Se deshabilitan los colliders de los nodos
        manager.OnWinMethod(); //Se chequea si el otro tracker también ganó
    }

    void DisableNodes()
    {
        foreach (var node in actionNodes)
        {
            node.GetComponent<BoxCollider>().enabled = false;
        }
    }

    private void AddNode(Node node)
    {
        currentPath.Add(node);
        var nodeMesh = node.nodeMesh;
        currentMeshes.Add(nodeMesh);
        TurnOrRestartNodes(nodeMesh, true);
    }

    private void TurnOrRestartNodes(MeshRenderer mesh, bool state)
    {
        if (state && mesh != null)
        {
            mesh.enabled = true;
        }
        else
        {
            foreach (var item in currentMeshes)
            {
                if (item != null)
                {
                    item.enabled = false;
                }
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