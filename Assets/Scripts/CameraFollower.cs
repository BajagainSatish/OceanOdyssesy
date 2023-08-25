using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class CameraFollower : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float damping = 0.5f;
    private Vector3 velocity = Vector3.zero;


    private void Update()
    {
        if (target == null)
            return;
        Vector3 movePosition = target.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, movePosition, ref velocity, damping);
    }
}