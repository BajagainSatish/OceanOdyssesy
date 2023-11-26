using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowerTarget : MonoBehaviour
{
    [SerializeField] private Vector3 defaultCameraOffset;   //the original offset value -- use this to allow object to move back to its default value
    [SerializeField] private Vector3 defaultCameraRotation;

    [HideInInspector] public Vector3 cameraOffset;    //the offset value that is set now //this is used by cameraFollower as the offset
    [HideInInspector] public Quaternion cameraRotation;
    public float cameraDamping;
    public Transform camTarget;

    private void Awake()
    {
        SetDefaultCameraTargetPosition();
        if (camTarget == null)
            camTarget = transform;
    }

    public void SetDefaultCameraTargetPosition()
    {
        cameraOffset = defaultCameraOffset;
        cameraRotation = Quaternion.Euler(defaultCameraRotation);
    }

    public void SetFromCurrentCameraPosition()
    {
        cameraOffset = Camera.main.transform.position - camTarget.position;
        cameraRotation = Camera.main.transform.rotation;
    }
}
