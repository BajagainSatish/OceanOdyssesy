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

    public void OnFocus()
    {
        controller.OnFocusableObjectFocus(this);
    }
}
