using UnityEngine;

public class WallLaser : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private LayerMask blockLaserLayer;
    [SerializeField] private float laserMaxDistance = 50f;

    private void Start()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.02f;
        lineRenderer.endWidth = 0.02f;
        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        lineRenderer.material.color = Color.red;
    }

    private void Update()
    {
        ShootLaser();
    }

    private void ShootLaser()
    {
        Debug.Log("Laser");

        Vector3 origin = transform.position;
        Vector3 direction = (target.position - origin).normalized;

        lineRenderer.SetPosition(0, origin);

        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, laserMaxDistance))
        {
            lineRenderer.SetPosition(1, hit.point);

            Debug.Log("Golpeé: " + hit.collider.name);

            if (((1 << hit.collider.gameObject.layer) & blockLaserLayer) != 0)
            {
                Debug.Log("¡Golpeé un bloqueador!");
                return;
            }

            Interactor interactor = hit.collider.GetComponent<Interactor>();

            if (interactor != null)
            {
                Debug.Log("¡Interact encontrado!");
                interactor.Interact();
            }
            else
            {
                Debug.Log("No encontré ningún Interactor en: " + hit.collider.name);
            }
        }
        else
        {
            lineRenderer.SetPosition(1, target.position);
        }
    }
}
