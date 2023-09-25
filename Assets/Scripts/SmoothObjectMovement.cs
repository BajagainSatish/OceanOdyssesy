using System.Collections;
using UnityEngine;

public class SmoothObjectMovement : MonoBehaviour
{
    public float moveThreshold = 0.05f;
    public float rotateThreshold = 1f;
    public float damping = 0.5f;
    public float rotationSpeed = 1f;
    private Vector3 velocity;

    public delegate void OnFinishCallback();

    private static void AddThisComponentTo(GameObject gameObject)
    {
        SmoothObjectMovement mover = gameObject.GetComponent<SmoothObjectMovement>();
        if (mover == null)
            gameObject.AddComponent<SmoothObjectMovement>();
    }

    public static void MoveObjectTo(GameObject gameObject, Vector3 newpos, OnFinishCallback callbackFunction = null)
    {
        AddThisComponentTo(gameObject);
        SmoothObjectMovement mover = gameObject.GetComponent<SmoothObjectMovement>();
        mover.MoveTo(newpos, callbackFunction);
    }
    public static void RotateObjectTo(GameObject gameObject, Quaternion newRot, OnFinishCallback callbackFunction = null)
    {
        AddThisComponentTo(gameObject);
        SmoothObjectMovement mover = gameObject.GetComponent<SmoothObjectMovement>();
        mover.RotateTo(newRot, callbackFunction);
    }

    public void MoveTo(Vector3 newpos, OnFinishCallback callbackFunction = null)
    {
        StopCoroutine("moveTo");
        StartCoroutine(moveTo(newpos, callbackFunction));
    }
    IEnumerator moveTo(Vector3 newpos, OnFinishCallback callbackFunction = null)
    {
        while((transform.position - newpos).magnitude > moveThreshold)
        {
            transform.position = Vector3.SmoothDamp(transform.position, newpos, ref velocity, damping);
            yield return null;
        }
        if(callbackFunction != null)
            callbackFunction();
    }

    public void RotateTo(Quaternion newRot, OnFinishCallback callbackFunction = null)
    {
        StopCoroutine("rotateTo");
        StartCoroutine(rotateTo(newRot, callbackFunction));
    }
    IEnumerator rotateTo(Quaternion newRot, OnFinishCallback callbackFunction = null)
    {
        while (Quaternion.Angle(transform.rotation, newRot) > rotateThreshold)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, newRot, rotationSpeed * Time.deltaTime);
            yield return null;
        }
        if (callbackFunction != null)
            callbackFunction();
    }
}
