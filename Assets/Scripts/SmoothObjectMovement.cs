using System.Collections;
using UnityEngine;

public class SmoothObjectMovement : MonoBehaviour
{
    public float moveThreshold = 0.05f;
    public float rotateThreshold = 0.5f;
    public float damping = 0.5f;
    public float rotationSpeed = 1f;
    private Vector3 velocity;

    private static void AddThisComponentTo(GameObject gameObject)
    {
        SmoothObjectMovement mover = gameObject.GetComponent<SmoothObjectMovement>();
        if (mover == null)
            gameObject.AddComponent<SmoothObjectMovement>();
    }

    public static void MoveObjectTo(GameObject gameObject, Vector3 newpos)
    {
        AddThisComponentTo(gameObject);
        SmoothObjectMovement mover = gameObject.GetComponent<SmoothObjectMovement>();
        mover.MoveTo(newpos);
    }
    public static void RotateObjectTo(GameObject gameObject, Quaternion newRot)
    {
        AddThisComponentTo(gameObject);
        SmoothObjectMovement mover = gameObject.GetComponent<SmoothObjectMovement>();
        mover.RotateTo(newRot);
    }

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
