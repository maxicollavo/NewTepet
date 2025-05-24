using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempManager : MonoBehaviour
{
    [SerializeField] GameObject objectOne;
    [SerializeField] GameObject objectTwo;
    [SerializeField] GameObject objectThree;

    [SerializeField] bool isLeft;

    [SerializeField] Transform leftPivot;
    [SerializeField] Transform rightPivot;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isLeft = !isLeft;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (isLeft)
                Instantiate(objectOne, leftPivot.position, Quaternion.identity);
            else
                Instantiate(objectOne, rightPivot.position, Quaternion.identity);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (isLeft)
                Instantiate(objectTwo, leftPivot.position, Quaternion.identity);
            else
                Instantiate(objectTwo, rightPivot.position, Quaternion.identity);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (isLeft)
                Instantiate(objectThree, leftPivot.position, Quaternion.identity);
            else
                Instantiate(objectThree, rightPivot.position, Quaternion.identity);
        }
    }
}
