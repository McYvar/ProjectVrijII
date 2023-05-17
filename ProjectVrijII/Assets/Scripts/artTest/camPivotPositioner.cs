using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camPivotPositioner : MonoBehaviour
{
    public GameObject moveAroundThis;

    void Update()
    {
        transform.position = moveAroundThis.transform.position;
    }
}
