using UnityEngine;

public class RotateCameraLoop : MonoBehaviour
{
    [Tooltip("Velocidad de rotación en grados por segundo")]
    [SerializeField] private float rotationSpeed = 10f;

    void Update()
    {
        // Rota suavemente en el eje Y (hacia la derecha)
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
    }
}
