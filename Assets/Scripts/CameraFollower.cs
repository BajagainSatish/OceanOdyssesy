using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class CameraFollower : MonoBehaviour
{
    private Vector3 velocity = Vector3.zero;
    public CameraFollowerTarget target;


    private void Update()
    {
        if (target == null)
            return;
        Vector3 movePosition = target.camTarget.position + target.cameraOffset;
        transform.position = Vector3.SmoothDamp(transform.position, movePosition, ref velocity, target.cameraDamping);
    }
}