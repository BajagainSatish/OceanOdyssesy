using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceFromCam : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            float distance = (transform.position - Camera.main.transform.position).magnitude;
            Debug.Log("Dist:" + distance);        }
    }
}
