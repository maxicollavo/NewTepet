using UnityEngine;

public class FollowMouseClick : MonoBehaviour
{
    private Camera cam;
    float fixedZ;

    private void Start()
    {
        cam = Camera.main;

        fixedZ = transform.position.z;

        gameObject.SetActive(false);
    }

    void Update()
    {
        if (!gameObject.activeInHierarchy) return;

        if (Input.GetMouseButton(0))
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Mathf.Abs(cam.transform.position.z - fixedZ);

            Vector3 worldPos = cam.ScreenToWorldPoint(mousePos);
            worldPos.z = fixedZ;

            transform.position = worldPos;
        }
    }
}