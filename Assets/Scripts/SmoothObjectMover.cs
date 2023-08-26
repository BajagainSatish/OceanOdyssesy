using System.Collections;
using UnityEngine;

public class SmoothObjectMover : MonoBehaviour
{
    public float moveThreshold = 0.05f;
    public float rotateThreshold = 0.5f;
    public float damping = 0.5f;
    public float rotationSpeed = 10f;
    private Vector3 velocity;

    public void MoveTo(Vector3 newpos)
    {
        StopCoroutine("moveTo");
        StartCoroutine("moveTo", newpos);
    }
    IEnumerator moveTo(Vector3 newpos)
    {
        while((transform.position - newpos).magnitude > moveThreshold)
        {
            transform.position = Vector3.SmoothDamp(transform.position, newpos, ref velocity, damping);
            yield return null;
        }
    }

    public void RotateTo(Quaternion newRot)
    {
        StopCoroutine("rotateTo");
        StartCoroutine("rotateTo", newRot);
    }
    IEnumerator rotateTo(Quaternion newRot)
    {
        while (Quaternion.Angle(transform.rotation, newRot) > 0.5f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, newRot, rotationSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
