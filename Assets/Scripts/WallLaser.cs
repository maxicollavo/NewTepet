using UnityEngine;

public class WallLaser : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] GameObject sphere;
    [SerializeField] SpherePuzzleManager manager;
    private Transform target;
    [SerializeField] private LayerMask blockLaserLayer;

    public bool isDefault;
    public bool isEnabled;

    private void OnEnable()
    {
        target = sphere.transform;
        ShootLaser();
        isEnabled = true;

        if (isDefault)
        {
            SetSphereMaterials();
        }

        if (sphere.TryGetComponent<RotateSphere>(out var sphereRot))
        {
            if (!sphereRot.hasWon) return;

            sphereRot.SetOnWinSphereMaterials();

            if (manager == null) return;

            manager.openDoor.SetTrigger("Open");
            
        }
    }


    private void SetSphereMaterials()
    {
        var renderer = sphere.GetComponent<MeshRenderer>();
        var materials = renderer.materials;

        materials[3].EnableKeyword("_EMISSION");

        Color originalEmission = materials[3].GetColor("_EmissionColor");
        materials[3].SetColor("_EmissionColor", originalEmission);

        Color glassColor = new Color(0f / 255f, 46f / 255f, 191f / 255f, 1f) * 4.816925f;
        materials[4].SetColor("_Color", glassColor);
        materials[4].SetFloat("_speed", 0.05f);

        renderer.materials = materials;
    }

    private void Start()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = 0.02f;
        lineRenderer.endWidth = 0.02f;
        lineRenderer.material = new Material(Shader.Find("Unlit/Color"));
        lineRenderer.material.color = Color.red;
    }

    private void ShootLaser()
    {
        if (target == null) return;

        Vector3 origin = transform.position;
        Vector3 targetPos = target.position;
        Vector3 direction = (targetPos - origin).normalized;
        float distanceToTarget = Vector3.Distance(origin, targetPos);

        if (Physics.Raycast(origin, direction, out RaycastHit hit, distanceToTarget, blockLaserLayer))
        {
            lineRenderer.SetPosition(0, origin);
            lineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            lineRenderer.SetPosition(0, origin);
            lineRenderer.SetPosition(1, targetPos);
        }
    }
}