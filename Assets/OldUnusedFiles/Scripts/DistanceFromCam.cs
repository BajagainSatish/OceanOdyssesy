using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceFromCam : MonoBehaviour
{
    public Transform target;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            float distance = (transform.position - Camera.main.transform.position).magnitude;
            Debug.Log("Dist:" + distance);       
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Vector3 offset = (-target.position + Camera.main.transform.position);
            Debug.Log("Offset:" + offset);
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            Debug.Log("CameraPos:" + Camera.main.transform.position);
        }
    }
}
