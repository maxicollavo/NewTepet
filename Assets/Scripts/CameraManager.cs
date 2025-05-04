using System;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class CameraManager : MonoBehaviour
{
    [SerializeField] float cameraLerpSpeed = 2f;
    [SerializeField] GameObject cameraParent;
    [SerializeField] GameObject playerParent;
    private Camera playerCam;

    private Vector3 originalCamPos;
    private Quaternion originalCamRot;

    private Transform targetPosition;
    private bool isLerpingToTarget;
    private bool isReturning;

    private bool currentState;

    private void Start()
    {
        playerCam = Camera.main;
    }

    public void LerpCamera(bool state, Transform waypoint)
    {
        if (waypoint == null || playerCam == null || currentState == state) return;

        currentState = state;

        if (state)
        {
            originalCamPos = playerCam.transform.position;
            originalCamRot = playerCam.transform.rotation;
            targetPosition = waypoint;
            isLerpingToTarget = true;
        }
        else
        {
            isReturning = true;
        }
    }

    private void Update()
    {
        if (isLerpingToTarget && targetPosition != null)
        {
            playerCam.transform.SetParent(cameraParent.transform);
            LerpCameraTo(targetPosition.position, targetPosition.rotation, ref isLerpingToTarget);
        }

        if (isReturning)
        {
            playerCam.transform.SetParent(playerParent.transform);
            LerpCameraTo(originalCamPos, originalCamRot, ref isReturning);
        }
    }

    private void LerpCameraTo(Vector3 position, Quaternion rotation, ref bool flag)
    {
        playerCam.transform.position = Vector3.Lerp(playerCam.transform.position, position, Time.deltaTime * cameraLerpSpeed);
        playerCam.transform.rotation = Quaternion.Lerp(playerCam.transform.rotation, rotation, Time.deltaTime * cameraLerpSpeed);

        float distance = Vector3.Distance(playerCam.transform.position, position);
        if (distance < 0.05f)
        {
            flag = false;
        }
    }
}
