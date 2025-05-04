using UnityEngine;

public class ScreamerTeleport : MonoBehaviour
{
    public Transform player;
    public float targetDistance = 2f;
    public float verticalOffset = 10f;

    public void TeleportToPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;

        Vector3 targetPosition = player.position - direction * targetDistance;
        targetPosition.y -= verticalOffset;

        transform.position = targetPosition;
        transform.position -= new Vector3(0,3f,0);
    }
}