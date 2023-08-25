using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusableObject : MonoBehaviour
{
    public Transform target;
    public float onFocusDistance;

    void Start()
    {
        if (target == null)
            target = transform;
    }

    public Vector3 GetFocusTargetPosition()
    {
        Vector3 cameraPos = Camera.main.transform.position;
        Vector3 targetDir = target.position - cameraPos;
        float distanceFactor = onFocusDistance/targetDir.magnitude;
        return Vector3.LerpUnclamped(target.position, cameraPos, distanceFactor);
    }
}
