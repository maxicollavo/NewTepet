using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] private Transform laserVisual;
    [SerializeField] private float maxDistance = 20f;
    [SerializeField] private LayerMask statueMask;

    private Vector3 originalScale;
    private MeshFilter meshFilter;
    private float meshHeight;

    private void Start()
    {
        originalScale = laserVisual.localScale;

        meshFilter = laserVisual.GetComponent<MeshFilter>();

        if (meshFilter != null)
        {
            meshHeight = meshFilter.sharedMesh.bounds.size.y;
        }
        else
        {
            meshHeight = 2f;
        }
    }

    private void Update()
    {
        Vector3 targetPoint = transform.position + transform.forward * maxDistance;

        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, maxDistance, statueMask))
        {
            LaserStatue statue = hit.collider.GetComponent<LaserStatue>();

            if (statue != null)
            {
                targetPoint = statue.laserPoint.position;

                Vector3 dir = statue.transform.forward;

                if (Physics.Raycast(statue.laserPoint.position, dir, out RaycastHit secondHit, maxDistance, statueMask))
                {
                    LaserStatue secondStatue = secondHit.collider.GetComponent<LaserStatue>();

                    if (secondStatue != null)
                    {
                        targetPoint = secondStatue.laserPoint.position;
                    }
                }
            }
        }

        UpdateLaser(targetPoint);
    }

    private void UpdateLaser(Vector3 targetPoint)
    {
        Vector3 startPoint = transform.position;
        Vector3 dir = targetPoint - startPoint;

        float distance = dir.magnitude;

        if (distance <= 0.01f) return;

        laserVisual.position = startPoint + dir.normalized * (distance / 2f);

        laserVisual.rotation = Quaternion.LookRotation(dir.normalized) * Quaternion.Euler(90f, 0f, 0f);

        laserVisual.localScale = new Vector3(
            originalScale.x,
            originalScale.y,
            originalScale.z
        );

        float currentLength = laserVisual.GetComponent<Renderer>().bounds.size.z;

        float multiplier = distance / currentLength;

        laserVisual.localScale = new Vector3(
            originalScale.x,
            originalScale.y * multiplier,
            originalScale.z
        );
    }
}