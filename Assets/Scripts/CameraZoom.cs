using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    public float zoomSpeed = 0.3f;
    //public float fieldOfViewMax = 50;
    //public float fieldOfViewMin = 10;

    public Transform target;
    public float ZoomInMinDistance;
    public float ZoomOutMaxDistance;

    private float previousBetweenFingerDistance;  //holds distance between two fingers used for zooming in previous frame

    private void Update()
    {
        if (Input.touchCount == 2)
        {
            Touch touch1 = Input.GetTouch(0);
            Touch touch2 = Input.GetTouch(1);
            if (touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began)
            {
                previousBetweenFingerDistance = (touch1.position - touch2.position).magnitude;
            }

            else
                HandleCameraZoom(touch1, touch2);
        }
    }
    private void HandleCameraZoom(Touch touch1, Touch touch2)
    {
        float currentBetweenFingerDistance = (touch1.position - touch2.position).magnitude;
        float deltaFingerDistance = previousBetweenFingerDistance - currentBetweenFingerDistance;
        float zoomDir;

        if (deltaFingerDistance == 0f)
            return;

        if (deltaFingerDistance > 0)
        {
            zoomDir = -1f;
        }
        else
        {
            zoomDir = 1f;
        }

        Vector3 newpos = Vector3.LerpUnclamped(transform.position, target.position, Time.deltaTime * zoomSpeed * zoomDir);
        float newCameraDistance = (newpos - target.position).magnitude;
        if (newCameraDistance > ZoomInMinDistance && newCameraDistance < ZoomOutMaxDistance)
            transform.position = newpos;

        previousBetweenFingerDistance = currentBetweenFingerDistance;
    }
}
