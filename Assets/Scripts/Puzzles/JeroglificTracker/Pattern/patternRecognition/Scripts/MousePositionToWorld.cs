using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePositionToWorld : MonoBehaviour
{
 public float ZDistance = 10f;


    // Update is called once per frame
    void Update()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector3(pos.x, pos.y, ZDistance);
    }
}
