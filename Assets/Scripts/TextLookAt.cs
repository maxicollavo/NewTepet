using UnityEngine;

public class TextLookAt : MonoBehaviour
{
    void Update()
    {
        transform.LookAt(Camera.main.transform);

        transform.Rotate(0, 180, 0);
    }
}
