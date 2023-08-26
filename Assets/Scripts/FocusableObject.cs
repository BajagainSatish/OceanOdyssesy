using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusableObject : MonoBehaviour
{
    public Transform target;
    public float onFocusDistance;

    GameController controller;

    void Start()
    {
        if (target == null)
            target = transform;
        controller = FindFirstObjectByType<GameController>();
    }

    public Vector3 GetFocusTargetPosition(Vector3 cameraPos)
    {
        Vector3 targetDir = target.position - cameraPos;
        float distanceFactor = onFocusDistance/targetDir.magnitude;
        return Vector3.LerpUnclamped(target.position, cameraPos, distanceFactor);
    }

    public Quaternion GetFocusTargetRotation(Transform cameraT)
    {
        Vector3 originalCamPos = cameraT.position;
        Quaternion originalCamRot = cameraT.rotation;
        Vector3 afterMovePos = GetFocusTargetPosition (originalCamPos);
        //get the rotation that would happen when the camera moves to target position and looks at the target object
        cameraT.position = afterMovePos;
        cameraT.LookAt(target.position);
        Quaternion afterMoveRot = cameraT.rotation;
        cameraT.position = originalCamPos;  //change back cameraPosition and rotation to their original values
        cameraT.rotation = originalCamRot;
        return afterMoveRot;
    }

    public void OnFocus()
    {
        controller.OnFocusableObjectFocus(this);
    }
}
