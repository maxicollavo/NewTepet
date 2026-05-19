using System.Collections;
using UnityEngine;

public class PlayerTeleport : MonoBehaviour
{
    private CharacterController characterController;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void OnEnable()
    {
        NewEventManager.OnChangeRoom += TeleportPlayer;
    }

    private void OnDisable()
    {
        NewEventManager.OnChangeRoom -= TeleportPlayer;
    }

    private void TeleportPlayer(Vector3 position, Quaternion rotation)
    {
        if (characterController != null)
            characterController.enabled = false;

        transform.position = position;
        transform.rotation = rotation;

        StartCoroutine(TeleportPlayerCoroutine());
    }

    private IEnumerator TeleportPlayerCoroutine()
    {
        yield return new WaitForSeconds(1f);
        if (characterController != null)
            characterController.enabled = true;
    }
}