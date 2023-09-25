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

    Coroutine moveCoroutine;
    Coroutine rotateCoroutine;

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
        StopMovement();
        moveCoroutine = StartCoroutine(moveTo(newpos, callbackFunction));
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
        StopRotation();
        rotateCoroutine = StartCoroutine(rotateTo(newRot, callbackFunction));
    }
    IEnumerator rotateTo(Quaternion newRot, OnFinishCallback callbackFunction = null)
    {
        while (Quaternion.Angle(transform.rotation, newRot) > rotateThreshold)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, newRot, rotationSpeed * Time.deltaTime);
            Debug.Log("inside: " + Quaternion.Angle(transform.rotation, newRot));
            yield return null;
        }
        if (callbackFunction != null)
            callbackFunction();
        Debug.Log("outside");
    }

    public void StopMovement()
    {
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);
    }
    public void StopRotation()
    {
        if (rotateCoroutine != null)
            StopCoroutine(rotateCoroutine);
    }

    public void StopAll()
    {
        StopAllCoroutines();
    }
}
