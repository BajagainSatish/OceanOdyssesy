using System.Collections;
using UnityEngine;

public class SmoothMover : MonoBehaviour
{
    public float moveThreshold = 0.05f;
    public float damping = 0.5f;
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
}
